// Este script maneja las interacciones del carrito de compras, incluyendo agregar, eliminar y actualizar productos en el carrito.
const cartModal = document.getElementById('CarritoModal');
const showCartBtn = document.getElementById('showCart');
const closeCartBtn = document.getElementById('closeCart');
const modalContent = cartModal.querySelector('.modalContent');
const formatter = new Intl.NumberFormat('es-ES', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
});
const cerrarSesionBtn = document.querySelector('.btnLogOut');

document.addEventListener('DOMContentLoaded', () => {

    if (cerrarSesionBtn) {
        $(document).ready(function () {

            //Obtener el antiforgery token si es necesario
            const antiforgeryToken = $('input[name="__RequestVerificationToken"]').val();
            // Inicializar el modal del carrito
            // verificar si el botón para mostrar el carrito existe
            if (showCartBtn) {
                showCartBtn.addEventListener('click', function () {
                    // verificar si el modal existe
                    if (cartModal) {
                        cartModal.showModal();
                        //Se hace una petición para obtener los productos del carrito
                        fetch('/Ventas/GetCartItems', {
                            method: 'GET',
                            headers: {'Accept': 'application/json'}
                        })
                            // Verificar si la respuesta es exitosa y la convertimos a JSON
                            .then(response => response.json())
                            .then(data => {
                                // Verificar si la respuesta contiene los datos esperados
                                if (data && data.success) {
                                    // Actualizar la información del carrito
                                    updateCartCount(data.cartItemCount, data.cartTotal);
                                    renderCartItems(data.cartItems);
                                } else {
                                    console.error('Error al obtener los productos del carrito:', data.message);
                                    showAlert('Error al cargar los productos del carrito.', 'error');
                                }
                            })
                            .catch((error) => {
                                console.error('Error al obtener los productos del carrito:', error);
                                if (!error === 'Usuario no autenticado.'){
                                    showAlert('Error al cargar los productos del carrito.', 'error');
                                }
                            })
                    }
                });
            }

            //Cargar de forma inicial los productos del carrito
            getCartItems()

            //Se asigna el evento de agregar al carrito a todos los botones con la clase btnAddToCart
            $('body').on('click', '.btnAddToCart', function () {
                const clickedButton = $(this); // $(this) is the button that was clicked
                const productId = clickedButton.data('productid');
                const quantity = clickedButton.data('quantity'); // Assuming you want quantity

                const numericProductId = parseInt(productId);
                const numericQuantity = parseInt(quantity);

                if (!isNaN(numericProductId) && !isNaN(numericQuantity)) {
                    // Call your existing addToCart function
                    addToCart(numericProductId, numericQuantity);

                } else {
                    console.error("Error: Invalid product ID or quantity for adding to cart.");
                }
            });

            //Se asigna el evento de eliminar del carrito a todos los botones con la clase remove-from-cart-btn
            $('body').on('click', '.remove-from-cart-btn', function () {
                const clickedButton = $(this).closest('.remove-from-cart-btn');

                // *** CAMBIO CLAVE AQUÍ: 'productid' en minúsculas ***
                const productId = clickedButton.data('productid');

                console.log("Producto ID obtenido del botón (corregido camelCase):", productId);

                const numericProductId = parseInt(productId);

                if (isNaN(numericProductId)) {
                    console.error("Error: productId no es un número válido. Valor:", productId);
                    showAlert('Error: ID de producto no válido.', 'error');
                    return;
                }

                removeFromCart(numericProductId);
            });

            //Se asigna el evento de vaciar el carrito a todos los botones con la clase btnClearCart
            $('body').on('click', '.btnClearCart', function () {
                emptyCart();
            });

            // Event listener for the "plus" button (increase quantity)
            $('body').on('click', '.btnPlus', function () {
                const productId = $(this).data('productid');
                const numericProductId = parseInt(productId);


                if (!isNaN(numericProductId)) {
                    // Call addToCart with quantity 1 to increment
                    addToCart(numericProductId, 1);
                } else {
                    console.error("Error: Product ID is not a valid number for increasing quantity.");
                }
            });

            // Event listener for the "minus" button (decrease quantity)
            $('body').on('click', '.btnMinus', function () {
                const productId = $(this).data('productid');
                const numericProductId = parseInt(productId);

                if (!isNaN(numericProductId)) {
                    // You'll need a separate function for decreasing or modify removeFromCart
                    decreaseCartItemQuantity(numericProductId, 1);
                } else {
                    console.error("Error: Product ID is not a valid number for decreasing quantity.");
                }
            });

            // Event listener para el botón de aplicar cupón
            $('body').on('click', '#btnAplicarCupon', function () {
                const couponCode = $('#discountCode').val(); // Obtiene el valor del input del cupón

                // Obtener el total actual del carrito desde el span de la vista de detalles
                // Limpia el formato de moneda y convierte a número flotante
                const currentCartTotalElement = $('#total-Cart-details');
                const currentCartTotal = parseFloat(currentCartTotalElement.data('productTotal'));

                if (!couponCode) {
                    showAlert('Por favor, introduzca un código de cupón.', 'warning');
                    return;
                }
                if (isNaN(currentCartTotal) || currentCartTotal <= 0) {
                    showAlert('El carrito está vacío o el total no es válido para aplicar un cupón.', 'warning');
                    return;
                }

                // Realizar la llamada AJAX al controlador
                fetch('/Ventas/ApplyDiscount', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json',
                    },
                    body: JSON.stringify({
                        codigoCupon: couponCode,
                        totalCarrito: currentCartTotal // Se envía el total, pero el servidor recalcula
                    })
                })
                    .then(response => {
                        if (!response.ok) {
                            // Manejar errores HTTP (ej. 400, 500)
                            return response.json().then(err => {
                                throw new Error(err.message || `Error del servidor: ${response.status}`);
                            });
                        }
                        return response.json();
                    })
                    .then(data => {
                        if (data.success) {
                            // Actualizar el span del total final con el nuevo total con descuento
                            $('#total-Cart-details-Final').text(`₡ ${formatter.format(data.totalCarritoConDescuento)}`).data('productTotal', data.totalCarritoConDescuento);
                            // Actualizar el span del descuento aplicado
                            $('#total-cart-descuento').text(`₡ ${formatter.format(data.descuentoAplicado)}`).data('discountValue', data.descuentoAplicado);

                            // Se actualiza los contadores y totales de la cabecera/modal si están presentes
                            updateCartCount(data.cartItemCount, data.totalCarritoConDescuento);

                            // Opcional: limpiar el campo del cupón si el descuento fue exitoso
                            $('#discountCode').val('');

                        } else {
                            showAlert(data.message, 'error');
                            // Limpiar el descuento si el cupón no es válido
                            $('#total-cart-descuento').text('₡ 0.00').data('discountValue', 0);

                            // Si hay un error, el total final debería volver al total sin descuento (o al último total válido)
                            // Para esto, se debe usar el valor original sin descuento del data-product-total
                            const originalTotal = parseFloat($('#total-Cart-details').data('productTotal'));
                            $('#total-Cart-details-Final').text(`₡ ${formatter.format(originalTotal)}`).data('productTotal', originalTotal);


                        }
                    })
                    .catch(error => {
                        console.error('Error al aplicar el cupón:', error);
                        showAlert('Error de conexión al intentar aplicar el cupón. Por favor, inténtelo de nuevo.', 'error');
                        // Limpiar el descuento y revertir el total si hay un error de conexión
                        $('#total-cart-descuento').text('₡ 0.00').data('discountValue', 0);

                        const originalTotal = parseFloat($('#total-Cart-details').data('productTotal'));
                        $('#total-Cart-details-Final').text(`₡ ${formatter.format(originalTotal)}`).data('productTotal', originalTotal);
                    });
            });

            // Event listener para el botón de limpiar cupón
            $('body').on('click', '#btnLimpiarCupon', function() {
                // Verificar que el token exista
                // Asegúrate de que el token existe
                if (!antiforgeryToken) {
                    console.error("Error: Anti-forgery token not found.");
                    showAlert('Error de seguridad. Recargue la página e inténtelo de nuevo.', 'error');
                    return;
                }

                // fetch para limpiar el cupón
                fetch(`/Ventas/RemoveDiscount`, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        'Accept': 'application/json',
                    },
                })
                    .then(response => {
                        // Manejar respuestas que no sean 200 OK (ej. 400, 404, 500)
                        if (!response.ok) {
                            // Leer la respuesta como texto para evitar SyntaxError si no es JSON
                            return response.text().then(text => {
                                throw new Error(text || `Error del servidor: ${response.status} ${response.statusText}`);
                            });
                        }
                        return response.json();
                    })
                    .then(data => {
                        if (data.success) {
                            // Actualizar el span del descuento a 0 y su data attribute
                            $('#total-cart-descuento').text('₡ 0.00').data('discountValue', 0);

                            // Volver a cargar los ítems del carrito para recalcular el total original
                            getCartItems();

                            showAlert('Cupón limpiado. El total ha sido recalculado.', 'info');
                        }else{
                            showAlert(data.message || 'Error al limpiar el cupón.', 'error');
                        }
                    }).catch(error => {
                    console.log(error);
                    showAlert('Error al limpiar el cupón. Por favor, inténtelo de nuevo.', 'error');
                })
            });
        });   
    }
});

//Función para actualizar el contador del carrito y el total
function updateCartCount(newCount, newTotal) {
    // Usar jQuery para seleccionar el elemento y se actualiza el texto de modal
    const cartItemCount = $('#cart-item-count');
    cartItemCount.text(newCount);

    const totalCart = $('#total-Cart');
    totalCart.text(`₡ ${formatter.format(newTotal)}`);

    // Actualizar el total en la vista de detalles del carrito
    const totalCartDetails = $('#total-Cart-details');
    totalCartDetails.text(`₡ ${formatter.format(newTotal)}`).data('productTotal', newTotal);
    // Actualizar el total final del carrito en la vista de detalles
    const totalCartDetailsFinal = $('#total-Cart-details-Final');
    totalCartDetailsFinal.text(`₡ ${formatter.format(newTotal)}`);

}

//Renderizar o rerenderizar la lista del productos del carrito
function renderCartItems(cartItems) {
    // Para el modal del carrito
    const cartItemListContainerModal = $('#cart-item-list-container'); // Este es el contenedor del modal
    cartItemListContainerModal.empty(); // Limpiar la lista actual

    if (cartItems && cartItems.length > 0) {
        cartItems.forEach(item => {
            const subtotal = item.quantity * item.productPrice;
            const rowModal = `
                    <div class="tableRow tCart" data-product-id="${item.productId}">
                        <span class="tableCell">${item.productName}</span>
                        <span class="tableCell">₡ ${formatter.format(item.productPrice)}</span>
                        <div class="tableCell columnCant">
                            <button class="btnMinus" data-productid="${item.productId}">
                                <img src="${item.minusImage}" alt="-">
                            </button>
                            <input type="number" 
                                   class="inputBase cantInput cart-item-quantity" 
                                   value="${item.quantity}" 
                                   min="1" 
                                   max="${item.productMaxStock}"
                                   data-productid="${item.productId}"
                                   readonly />
                            <button class="btnPlus" data-productid="${item.productId}">
                                <img src="${item.plusImage}" alt="+">
                            </button>
                        </div>
                        <span class="tableCell cart-item-subtotal">₡ ${formatter.format(subtotal)}</span>
                        <div class="tableButtonsColumn">
                                <button type="button"
                                    class="DeleteBtn remove-from-cart-btn tooltipContainer btnSwicthActive redHighlight"
                                    data-productId="${item.productId}">
                                        <img src="${item.deleteImage}" alt="Eliminar" class="iconDeactivate"/>
                                        <span class="TooltipText transM50">Eliminar </span>
                                </button>
                        </div>
                    </div>`;
            cartItemListContainerModal.append(rowModal);
        });
    } else {
        cartItemListContainerModal.append('' +
            '<div id="empty-cart-message-modal" class="tableRow">' +
            '<span class="NoElements">No hay productos en el carrito.</span>' +
            '</div>');
    }

    // Actualizar los detalles del carrito en caso de que se esté en esa pestaña
    const cartItemListContainerDetails = $('#cart-item-list-container-details');
    if (cartItemListContainerDetails.length) { // Verificar si el contenedor de detalles existe
        cartItemListContainerDetails.empty(); // Limpiar la lista actual de detalles

        if (cartItems && cartItems.length > 0) {
            cartItems.forEach(item => {
                const subtotal = item.quantity * item.productPrice;
                const rowDetails = `
                    <div class="tableRow TCartDetails" data-product-id="${item.productId}">
                        <span class="tableCell">${item.productName}</span>
                        <span class="tableCell">₡ ${formatter.format(item.productPrice)}</span>
                        <div class="tableCell columnCant">
                            <button class="btnMinus" data-productid="${item.productId}">
                                <img src="${item.minusImage}" alt="-">
                            </button>
                            <input type="number"
                                   class="inputBase cantInput cart-item-quantity"
                                   value="${item.quantity}"
                                   min="1"
                                   max="${item.productMaxStock}"
                                   data-productid="${item.productId}"
                                   readonly /> <button class="btnPlus" data-productid="${item.productId}">
                                <img src="${item.plusImage}" alt="+">
                            </button>
                        </div>
                        <span class="tableCell">₡ ${formatter.format(subtotal)}</span>
                        <div class="tableButtonsColumn">
                            <button type="button"
                                    class="DeleteBtn remove-from-cart-btn tooltipContainer btnSwicthActive redHighlight"
                                    data-productId="${item.productId}">
                                <img src="${item.deleteImage}" alt="Eliminar" class="iconDeactivate"/>
                                <span class="TooltipText transM50">Eliminar </span>
                            </button>
                        </div>
                    </div>
                `;

                cartItemListContainerDetails.append(rowDetails);
            });
        } else {
            cartItemListContainerDetails.append('' +
                '<div class="tableRow">' +
                '<span class="NoElements">No hay productos en el carrito.</span>' +
                '</div>');
        }
    }
}

function getCartItems(){
    fetch('/Ventas/GetCartItems', {
        method: 'GET',
        headers: {'Accept': 'application/json'}
    })
        .then(response => response.json())
        .then(data => {
            if (data && data.success) {
                updateCartCount(data.cartItemCount, data.cartTotal);
                renderCartItems(data.cartItems);

                // Cuando se carga el carrito, el total final (sin cupón) será el mismo que el total de productos.
                $('#total-Cart-details').text(`₡ ${formatter.format(data.subtotalCart)}`).data('productTotal', data.subtotalCart);

                // Actualizar el total final del carrito en la vista de detalles
                $('#total-Cart-details-Final').text(`₡ ${formatter.format(data.cartTotal)}`).data('productTotal', data.cartTotal);

                // Actualizar el "Descuento"
                $('#total-cart-descuento').text(`₡ ${formatter.format(data.descuentoAplicado)}`).data('discountValue', data.descuentoAplicado);

                // Si hay un cupón aplicado, mostrarlo en el input
                if (data.appliedCouponCode) {
                    $('#discountCode').val(data.appliedCouponCode);
                } else {
                    $('#discountCode').val(''); // Limpiar el input si no hay cupón aplicado
                }

            } else {
                console.error('Error al obtener los productos del carrito:', data.message);
                showAlert('Error al cargar los productos del carrito.', 'error');
                // Opcional: limpiar los totales si la carga falla
                updateCartCount(0, 0); // Limpia el contador y total del modal
                $('#total-Cart-details').text('₡ 0.00').data('productTotal', 0);
                $('#total-Cart-details-Final').text('₡ 0.00').data('productTotal', 0);
                $('#total-cart-descuento').text('₡ 0.00').data('discountValue', 0);
                $('#discountCode').val('');
            }
        })
        .catch((error) => {
            console.error('Error al obtener los productos del carrito:', error);
            showAlert('Error al cargar los productos del carrito.', 'error');
        }); 
}

function removeFromCart(productId) {
    fetch("/Ventas/DeleteCartItem", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'accept': 'application/json'
        },
        body: JSON.stringify({ productId: productId })
    }).then(res => {
        if (!res.ok) {
            throw new Error(`HTTP error! status: ${res.status}`);
        }
        return res.json();
    })
        .then(data => {
            if (data.success) {
                showAlert(data.message, 'success');
                getCartItems();
            } else {
                showAlert(data.message, 'error');
            }
        })
        .catch(err => {
            console.error('Error removing from cart:', err);
            showAlert('Error al eliminar el producto del carrito.', 'error');
        });
}

function addToCart(productId, quantity = 1){
    fetch("/Ventas/AddToCart", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'accept': 'application/json'
        },
        body: JSON.stringify({
            productId: productId,
            quantity: quantity,
        })
    }).then(res => {
        if (!res.ok) {
            throw new Error(`HTTP error! status: ${res.status}`);
        }
        return res.json();
    })
        .then(data => {
            if (data.success) {
                showAlert(data.message, 'success');
                getCartItems();
            } else {
                showAlert(data.message, 'error');
            }
        })
        .catch(err => {
            console.error('Error adding to cart:', err);
            showAlert('Error al agregar el producto al carrito.', 'error');
        })
}

function decreaseCartItemQuantity(productId, quantityToDecrease = 1) {
    fetch("/Ventas/DecreaseCartItem", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'accept': 'application/json'
        },
        body: JSON.stringify({
            productId: productId,
            quantity: quantityToDecrease, // Pass how much to decrease by
        })
    }).then(res => {
        if (!res.ok) {
            throw new Error(`HTTP error! status: ${res.status}`);
        }
        return res.json();
    })
        .then(data => {
            if (data.success) {
                // After successful update, refresh cart items
                getCartItems();
            } else {
                showAlert(data.message, 'error');
            }
        })
        .catch(err => {
            console.error('Error decreasing cart item quantity:', err);
            showAlert('Error al disminuir la cantidad del producto en el carrito.', 'error');
        });
}

function emptyCart() {
    fetch("/Ventas/EmptyCart", {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'accept': 'application/json'
        }
    }).then(res => {
        if (!res.ok) {
            throw new Error(`HTTP error! status: ${res.status}`);
        }
        return res.json();
    })
        .then(data => {
            if (data.success) {
                showAlert(data.message, 'success');
                updateCartCount(0, "0.00");
                renderCartItems([], "0.00")
            } else {
                showAlert(data.message, 'error');
            }
        })
        .catch(err => {
            console.error('Error vaciando el carrito:', err);
            showAlert('Error al vaciar el carrito.', 'error');
        });
}
