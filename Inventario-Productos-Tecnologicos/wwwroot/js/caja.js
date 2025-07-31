const EndCompraModal = document.getElementById('EndCompraModal');
const EndCompraModalContent = EndCompraModal.querySelector('.EndCompraModalContent');
const showCheckoutBtn = document.getElementById('showCheckout');
const closeCheckoutBtn = document.getElementById('closeCheckout');
const formatter = new Intl.NumberFormat('es-ES', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
});

const searchProductDialog = $('#searchProductDialog');
const searchProductDialogContent = searchProductDialog.find('.searchProductDialogContent');
const searchProductTblContent = searchProductDialog.find('.tbl-content');
const searchProductInput = $('#searchProductInput');
const btnSearchProduct = $('#btnSearchProduct');

document.addEventListener('DOMContentLoaded', () => {
    $(document).ready(() => {
        //Obtener el antiforgery token si es necesario
        const antiforgeryToken = $('input[name="__RequestVerificationToken"]').val();
        
        initSearchProductModal();

        $('body').on('click', '#btnExitModalSearch', () => {
            closeModalAnimation(searchProductDialogContent[0], searchProductDialog[0]);
        });

        //Se asigna el evento de vaciar el carrito a todos los botones con la clase btnClearCart
        $('body').on('click', '.btnClearCaja', function () {
            emptyCart();
        });

        //Inicializar el modal de finalización de compra
        initEndCompraModal();

        //Cargar de forma inicial los productos de la caja
        getCajaItems()
        


        //Se asigna el evento de agregar al carrito a todos los botones con la clase btnAddToCart
        $('body').on('click', '.btnAddToCaja', function () {
            const clickedButton = $(this); // $(this) is the button that was clicked
            // 1. Accede al input por su ID
            const productIdInput = $('#productId');
            console.log(productIdInput);
            // 2. Obtén el valor de ese input
            const productIdFromInput = productIdInput.val()
            console.log(productIdFromInput);
            
            const numericProductId = parseInt(productIdFromInput);
            console.log(numericProductId);

            if (!isNaN(numericProductId)) {
                // Call your existing addToCart function
                AddProductToCaja(numericProductId, 1);
                productIdInput.val("")

            } else {
                console.error("Error: Invalid product ID or quantity for adding to cart.");
            }
        });


        //Se asigna el evento de eliminar del carrito a todos los botones con la clase remove-from-cart-btn
        $('body').on('click', '.remove-from-caja-btn', function () {
            const clickedButton = $(this).closest('.remove-from-caja-btn');

            // *** CAMBIO CLAVE AQUÍ: 'productid' en minúsculas ***
            const productId = clickedButton.data('productid');

            console.log("Producto ID obtenido del botón (corregido camelCase):", productId);

            const numericProductId = parseInt(productId);

            if (isNaN(numericProductId)) {
                console.error("Error: productId no es un número válido. Valor:", productId);
                showAlert('Error: ID de producto no válido.', 'error');
                return;
            }

            removeFromCaja(numericProductId);
        });

        // Event listener for the "plus" button (increase quantity)
        $('body').on('click', '.btnPlus', function () {
            const productId = $(this).data('productid');
            const numericProductId = parseInt(productId);


            if (!isNaN(numericProductId)) {
                // Call addToCart with quantity 1 to increment
                AddProductToCaja(numericProductId, 1);
            } else {
                console.error("Error: Product ID is not a valid number for increasing quantity.");
            }
        });

        // Event listener for the "minus" button (decrease quantity)
        $('body').on('click', '.btnMinus', function () {
            const productId = $(this).data('productid');
            const numericProductId = parseInt(productId);
            
            console.log(numericProductId);

            if (!isNaN(numericProductId)) {
                // You'll need a separate function for decreasing or modify removeFromCart
                decreaseCajaItemQuantity(numericProductId, 1);
            } else {
                console.error("Error: Product ID is not a valid number for decreasing quantity.");
            }
        });

        // Event listener para el botón de aplicar cupón
        $('body').on('click', '#btnAplicarCupon', function () {
            const couponCode = $('#discountCode').val(); // Obtiene el valor del input del cupón

            // Obtener el total actual del carrito desde el span de la vista de detalles
            // Limpia el formato de moneda y convierte a número flotante
            const currentCajaTotalElement = $('#total-Caja-details');
            const currentCajaTotal = parseFloat(currentCajaTotalElement.data('productTotal'));

            if (!couponCode) {
                showAlert('Por favor, introduzca un código de cupón.', 'warning');
                return;
            }
            if (isNaN(currentCajaTotal) || currentCajaTotal <= 0) {
                showAlert('La caja está vacía o el total no es válido para aplicar un cupón.', 'warning');
                return;
            }

            // Realizar la llamada AJAX al controlador
            fetch('/Caja/ApplyDiscount', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                },
                body: JSON.stringify({
                    codigoCupon: couponCode,
                    totalCarrito: currentCajaTotal // Se envía el total, pero el servidor recalcula
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
                        $('#total-Caja-details-Final').text(`₡ ${formatter.format(data.totalCajaConDescuento)}`).data('productTotal', data.totalCajaConDescuento);
                        // Actualizar el span del descuento aplicado
                        $('#total-Caja-descuento').text(`₡ ${formatter.format(data.descuentoAplicado)}`).data('discountValue', data.descuentoAplicado);

                        // Se actualiza los contadores y totales de la cabecera/modal si están presentes
                        updateCajaCount(data.totalCajaConDescuento);

                        // Opcional: limpiar el campo del cupón si el descuento fue exitoso
                        $('#discountCode').val('');

                    } else {
                        showAlert(data.message, 'error');
                        // Limpiar el descuento si el cupón no es válido
                        $('#total-caja-descuento').text('₡ 0.00').data('discountValue', 0);

                        // Si hay un error, el total final debería volver al total sin descuento (o al último total válido)
                        // Para esto, se debe usar el valor original sin descuento del data-product-total
                        const originalTotal = parseFloat($('#total-Caja-details').data('productTotal'));
                        $('#total-Caja-details-Final').text(`₡ ${formatter.format(originalTotal)}`).data('productTotal', originalTotal);


                    }
                })
                .catch(error => {
                    console.error('Error al aplicar el cupón:', error);
                    showAlert('Error de conexión al intentar aplicar el cupón. Por favor, inténtelo de nuevo.', 'error');
                    // Limpiar el descuento y revertir el total si hay un error de conexión
                    $('#total-caja-descuento').text('₡ 0.00').data('discountValue', 0);

                    const originalTotal = parseFloat($('#total-Caja-details').data('productTotal'));
                    $('#total-Caja-details-Final').text(`₡ ${formatter.format(originalTotal)}`).data('productTotal', originalTotal);
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
            fetch(`/Caja/RemoveDiscount`, {
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
                        $('#total-caja-descuento').text('₡ 0.00').data('discountValue', 0);

                        // Volver a cargar los ítems del carrito para recalcular el total original
                        getCajaItems();

                        showAlert('Cupón limpiado. El total ha sido recalculado.', 'info');
                    }else{
                        showAlert(data.message || 'Error al limpiar el cupón.', 'error');
                    }
                }).catch(error => {
                console.log(error);
                showAlert('Error al limpiar el cupón. Por favor, inténtelo de nuevo.', 'error');
            })
        });

        const totalCajaCheckout = $('#total-Caja-checkout');
        const dineroEntregaClienteInput = $('#dineroEntregaCliente');
        const vueltoCajaInput = $('#vueltoCaja');
        const putTotalBtn = $('#putTotalBtn');

        // Función para calcular y mostrar el vuelto
        function calculateChange() {
            // Obtener el total de venta del atributo data-total (más preciso que del value con "₡ ")
            const totalVentaText = totalCajaCheckout.data('total'); // Obtiene el valor numérico sin el símbolo
            const totalVenta = parseFloat(totalVentaText);

            const dineroEntregado = parseFloat(dineroEntregaClienteInput.val());

            // Asegúrate de que los valores sean números válidos
            if (!isNaN(totalVenta) && !isNaN(dineroEntregado)) {
                let vuelto = dineroEntregado - totalVenta;

                // Si el dinero entregado es menor que el total, el vuelto es 0 o un mensaje de "Faltante"
                if (vuelto < 0) {
                    // Puedes mostrar "Faltante: ₡ X.XX" o simplemente "₡ 0.00"
                    vueltoCajaInput.val(`Faltan ₡ ${formatter.format(Math.abs(vuelto))}`);
                    // O si solo quieres mostrar el vuelto negativo, pero el usuario pidió "Vuelto"
                    // vueltoCajaInput.val(`₡ ${formatter.format(vuelto)}`);
                } else {
                    vueltoCajaInput.val(`₡ ${formatter.format(vuelto)}`);
                }
            } else {
                vueltoCajaInput.val('₡ 0.00'); // Resetear si los inputs no son válidos
            }
        }

        // Evento para el cambio en el campo "Dinero que entrega el cliente"
        dineroEntregaClienteInput.on('input', calculateChange);

        // Evento para el botón "Total" (putTotalBtn)
        putTotalBtn.on('click', () => {
            const totalVentaText = totalCajaCheckout.data('total'); // Obtiene el valor numérico
            const totalVenta = parseFloat(totalVentaText);

            if (!isNaN(totalVenta)) {
                dineroEntregaClienteInput.val(totalVenta); // Coloca el total de venta en el campo de entrega
                calculateChange(); // Recalcula el vuelto inmediatamente
            }
        });
    })
})

function decreaseCajaItemQuantity(productId, quantityToDecrease = 1) {
    fetch("/Caja/DecreaseCajaItem", {
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
                getCajaItems();
            } else {
                showAlert(data.message, 'error');
            }
        })
        .catch(err => {
            console.error('Error decreasing cart item quantity:', err);
            showAlert('Error al disminuir la cantidad del producto en el carrito.', 'error');
        });
}

function removeFromCaja(productId) {
    fetch("/Caja/DeleteCajaItem", {
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
                getCajaItems();
            } else {
                showAlert(data.message, 'error');
            }
        })
        .catch(err => {
            console.error('Error removing from cart:', err);
            showAlert('Error al eliminar el producto del carrito.', 'error');
        });
}


function initEndCompraModal() {
    //Manejar el cerrar el modal del carrito al presionar clic fuera del contenido del modal
    EndCompraModal.addEventListener('click', (e) => {
        const rect = EndCompraModalContent.getBoundingClientRect();
        const isInDialog = rect.top <= e.clientY && e.clientY <= rect.top + rect.height &&
            rect.left <= e.clientX && e.clientX <= rect.left + rect.width;
        if (!isInDialog) {
            closeModalAnimation(EndCompraModalContent, EndCompraModal);
        }
    });

    showCheckoutBtn.addEventListener('click', () => {
        showModal(EndCompraModal)
    })

    // Manejar el evento de clic en el botón de cerrar del carrito
    closeCheckoutBtn.addEventListener('click', () => {
        closeModalAnimation(EndCompraModalContent, EndCompraModal)
    });
}

function initSearchProductModal(){

    // Se carga el evento del doble click sobre el input donde se ingresan los código de los productos
    $('#productId').on('dblclick', (e) => {
        e.stopPropagation();
        showModal(searchProductDialog[0]);
        loadProductsForSearchModal('');
    });

    btnSearchProduct.on('click', () => {
        const searchTerm = searchProductInput.val();
        loadProductsForSearchModal(searchTerm);
    });

    searchProductInput.on('keypress', function(e) {
        if (e.which === 13) { // 13 es el código para la tecla Enter
            btnSearchProduct.click(); // Simula un clic en el botón de búsqueda
        }
    });

    $('body').on('click', '.selectSearchProductBtn', function() { 
        const productId = $(this).data('productid'); // 'this' ahora se refiere al botón clicado
        console.log("Producto ID seleccionado:", productId); // Para depuración

        if (!isNaN(parseInt(productId))) {
            AddProductToCaja(parseInt(productId), 1);
            closeModalAnimation(searchProductDialogContent[0], searchProductDialog[0]);
        } else {
            console.error("Error: Product ID no es válido o no se pudo obtener.");
            showAlert('Error: No se pudo seleccionar el producto. Intente de nuevo.', 'error');
        }
    });
}
function loadProductsForSearchModal(searchTerm = '') { // Parámetro con valor por defecto
    searchProductTblContent.empty().append('<div class="tableRow"><span class="NoElements">Cargando productos...</span></div>');

    // Construir la URL con el parámetro de búsqueda
    const url = `/Caja/GetProductToSearch?searchTerm=${encodeURIComponent(searchTerm)}`;

    fetch(url, {
        method: 'GET',
        headers: { 'Accept': 'application/json' }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error(`Error HTTP: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            searchProductTblContent.empty(); // Limpiar el mensaje de carga

            if (data && data.length > 0) {
                data.forEach(prod => {
                    const productRow = `
                    <div class="tableRow tSearchProduct">
                        <span class="tableCell">${prod.tn_Id}</span>
                        <span class="tableCell">${prod.tc_Nombre}</span>
                        <span class="tableCell centerTextCell">${prod.tn_Stock}</span>
                        <div class="tableButtonsColumn">
                            <button type="button" data-productId="${prod.tn_Id}" class="selectSearchProductBtn btnCreate tooltipContainer">
                                <img src="/img/ICO_Add.svg" alt="Seleccionar"/>
                                <span class="TooltipText">Seleccionar</span>
                            </button>
                        </div>
                    </div>
                `;
                    searchProductTblContent.append(productRow);
                });
                searchProductTblContent.append('<div class="tableRow listEnd"><span>Fin de la Lista</span></div>');
            } else {
                searchProductTblContent.append('<div class="tableRow"><span class="NoElements">No se encontraron productos.</span></div>');
            }
        })
        .catch(error => {
            console.error('Error al cargar productos para el modal de búsqueda:', error);
            searchProductTblContent.empty().append('<div class="tableRow"><span class="NoElements">Error al cargar productos. Por favor, inténtelo de nuevo.</span></div>');
            showAlert('Error al cargar los productos para la búsqueda.', 'error');
        });
}


function closeModalAnimation(modalContent, Modal) {
    console.log("Cerrando Modal")
    console.log(Modal);
    if (!modalContent) {
        return;
    }
    if (!modalContent.classList.contains('modal-fade-out')) {
        modalContent.classList.add('modal-fade-out');
        modalContent.addEventListener('animationend', () => {
            modalContent.classList.remove('modal-fade-out');
            Modal.close();
        }, {once: true});
    }
}

function showModal(modal) {
    if (modal && modal.showModal) {
        modal.showModal();
        modal.classList.remove('modal-fade-out');
    } else {
        console.error('El modal no es válido o no tiene el método showModal.');
    }
}

function updateCajaCount(newTotal) {
    console.log(`₡ ${formatter.format(newTotal)}`);
    const totalCaja = $('#total-Caja-checkout');
    totalCaja.val(`₡ ${formatter.format(newTotal)}`).data("total", newTotal);

    // Actualizar el total en la vista de detalles del carrito
    const totalCajaDetails = $('#total-Caja-details');
    totalCajaDetails.text(`₡ ${formatter.format(newTotal)}`).data('productTotal', newTotal);
    // Actualizar el total final del carrito en la vista de detalles
    const totalCajaDetailsFinal = $('#total-Caja-details-Final');

}

function renderCajaItems(cajaItems){
    const cajaItemListContainerDetails = $('#caja-item-list-container-details');
    if (cajaItemListContainerDetails.length) { // Verificar si el contenedor de detalles existe
        cajaItemListContainerDetails.empty(); // Limpiar la lista actual de detalles

        if (cajaItems && cajaItems.length > 0) {
            cajaItems.forEach(item => {
                const subtotal = item.quantity * item.productPrice;
                const rowDetails = `
                    <div class="tableRow TCajaDetails" data-product-id="${item.productId}">
                        <span class="tableCell">${item.productName}</span>
                        <span class="tableCell centerTextCell">₡ ${formatter.format(item.productPrice)}</span>
                        <div class="tableCell columnCant">
                            <button class="btnMinus" data-productid="${item.productId}">
                                <img src="${item.minusImage}" alt="-">
                            </button>
                            <input type="number"
                                   class="inputBase cantInput caja-item-quantity"
                                   value="${item.quantity}"
                                   min="1"
                                   max="${item.productMaxStock}"
                                   data-productid="${item.productId}"
                                   readonly /> <button class="btnPlus" data-productid="${item.productId}">
                                <img src="${item.plusImage}" alt="+">
                            </button>
                        </div>
                        <span class="tableCell centerTextCell">₡ ${formatter.format(subtotal)}</span>
                        <div class="tableButtonsColumn">
                            <button type="button"
                                    class="DeleteBtn remove-from-caja-btn tooltipContainer btnSwicthActive redHighlight"
                                    data-productId="${item.productId}">
                                <img src="${item.deleteImage}" alt="Eliminar" class="iconDeactivate"/>
                                <span class="TooltipText transM50">Eliminar </span>
                            </button>
                        </div>
                    </div>
                `;

                cajaItemListContainerDetails.append(rowDetails);
            });
        } else {
            cajaItemListContainerDetails.append('' +
                '<div class="tableRow">' +
                '<span class="NoElements">No hay productos en el carrito.</span>' +
                '</div>');
        }
    }
}

function getCajaItems(){
    fetch('/Caja/getCajaItems', {
        method: 'GET',
        headers: {'Accept': 'application/json'}
    }).then(response => response.json())
        .then(data =>{
            if(data && data.success){
                updateCajaCount(data.cajaTotal);
                renderCajaItems(data.cajaItems);
                // Cuando se carga el carrito, el total final (sin cupón) será el mismo que el total de productos.
                $('#total-Caja-details').text(`₡ ${formatter.format(data.subtotalCaja)}`).data('productTotal', data.subtotalCaja);

                // Actualizar el total final del carrito en la vista de detalles
                $('#total-Caja-details-Final').text(`₡ ${formatter.format(data.cajaTotal)}`).data('productTotal', data.cajaTotal);

                // Actualizar el "Descuento"
                $('#total-Caja-descuento').text(`₡ ${formatter.format(data.descuentoAplicado)}`).data('discountValue', data.descuentoAplicado);

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
                $('#total-Caja-details').text('₡ 0.00').data('productTotal', 0);
                $('#total-Caja-details-Final').text('₡ 0.00').data('productTotal', 0);
                $('#total-Caja-descuento').text('₡ 0.00').data('discountValue', 0);
                $('#discountCode').val('');
            }
        })
        .catch((error) => {
            console.error('Error al obtener los productos del carrito:', error);
            showAlert('Error al cargar los productos del carrito.', 'error');
        });
}



function AddProductToCaja(productId, quantity = 1) {
    console.log(productId);
    fetch('/Caja/AddProductToCaja', {
        method : 'POST',
        headers : { 'Accept': 'application/json', 'Content-Type': 'application/json' },
        body : JSON.stringify({
            productId: productId,
            quantity: quantity,
        })
    }).then(res =>{
        if (!res.ok){
            throw new Error(`HTTP error! status: ${res.status}`);
        }
        return res.json();

    }).then(data =>{
        if (data.success){
            showAlert(data.message, 'success');
            getCajaItems()
            
        }else{
            showAlert(data.message, 'error');
        }
    })
}

function emptyCart() {
    fetch("/Caja/EmptyCart", {
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
                updateCajaCount(0, "0.00");
                renderCajaItems([], "0.00")
            } else {
                showAlert(data.message, 'error');
            }
        })
        .catch(err => {
            console.error('Error vaciando el carrito:', err);
            showAlert('Error al vaciar el carrito.', 'error');
        });
}





