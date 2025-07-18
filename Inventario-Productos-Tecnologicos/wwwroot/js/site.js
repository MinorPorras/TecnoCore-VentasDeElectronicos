// Variables globales para el modal del carrito
const cartModal = document.getElementById('CarritoModal');
const showCartBtn = document.getElementById('showCart');
const closeCartBtn = document.getElementById('closeCart');
const modalContent = cartModal.querySelector('.modalContent');
const formatter = new Intl.NumberFormat('es-ES', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2
});

document.addEventListener('DOMContentLoaded', () => {
    if (document.querySelector('.register')) {
        initDropdownProvinciaCanton()
    }

    $(document).ready(function () {
        // Inicializar el modal del carrito
        // verificar si el botón para mostrar el carrito existe
        if (showCartBtn){
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
                            }else{
                                console.error('Error al obtener los productos del carrito:', data.message);
                                showAlert('Error al cargar los productos del carrito.', 'error');
                            }
                        })
                        .catch((error) => {
                            console.error('Error al obtener los productos del carrito:', error);
                            showAlert('Error al cargar los productos del carrito.', 'error');
                        })
                }
            });
        }

        // Asignar la animación de cierre al modal
        function closeModalAnimation() {
            if (!modalContent) {
                return;
            }
            if (!modalContent.classList.contains('modal-fade-out')) {
                modalContent.classList.add('modal-fade-out');
                modalContent.addEventListener('animationend', () => {
                    modalContent.classList.remove('modal-fade-out');
                    cartModal.close();
                }, {once: true});
            }
        }

        //Manejar el cerrar el modal del carrito al presionar clic fuera del contenido del modal
        cartModal.addEventListener('click', (e) => {
            const rect = modalContent.getBoundingClientRect();
            const isInDialog = rect.top <= e.clientY && e.clientY <= rect.top + rect.height &&
                rect.left <= e.clientX && e.clientX <= rect.left + rect.width;
            if (!isInDialog) {
                closeModalAnimation();
            }
        });

        // Manejar el evento de clic en el botón de cerrar del carrito
        closeCartBtn.addEventListener('click', () => {
            closeModalAnimation();
        });

        //Cargar de forma inicial los productos del carrito
        getCartItems()

        //Se asigna el evento de agregar al carrito a todos los botones con la clase btnAddToCart
        $('body').on('click', '.btnAddToCart', function() {
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
        $('body').on('click', '.remove-from-cart-btn', function() {
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
        $('body').on('click', '.btnClearCart', function() {
            emptyCart();
        });

        // Event listener for the "plus" button (increase quantity)
        $('body').on('click', '.btnPlus', function() {
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
        $('body').on('click', '.btnMinus', function() {
            const productId = $(this).data('productid');
            const numericProductId = parseInt(productId);

            if (!isNaN(numericProductId)) {
                // You'll need a separate function for decreasing or modify removeFromCart
                decreaseCartItemQuantity(numericProductId, 1);
            } else {
                console.error("Error: Product ID is not a valid number for decreasing quantity.");
            }
        });

    })
});

//Función para actualizar el contador del carrito y el total
function updateCartCount(newCount, newTotal) {
    // Usar jQuery para seleccionar el elemento y se actualiza el texto
    const cartItemCount = $('#cart-item-count');
    cartItemCount.text(newCount);

    console.log("Total nuevo: " + newTotal);
    const totalCart = $('#total-Cart');
    totalCart.text(`₡ ${formatter.format(newTotal)}`);
}

//Renderizar o rerenderizar la lista del productos del carrito
function renderCartItems(cartItems) {
    const cartItemListContainer = $('#cart-item-list-container');
    cartItemListContainer.empty(); // Limpiar la lista actual

    if (cartItems && cartItems.length > 0) {
        cartItems.forEach(item => {
            console.log(item);
            const subtotal = item.quantity * item.productPrice; // Parse to float for calculation
            const row = `
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
            cartItemListContainer.append(row);
        });
    } else {
        cartItemListContainer.append('' +
            '<div id="empty-cart-message-modal" class="tableRow">' +
                '<span class="NoElements">No hay productos en el carrito.</span>' +
            '</div>');
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
                console.log("cart Items: "+ data.cartItems)
                updateCartCount(data.cartItemCount, data.cartTotal);
                renderCartItems(data.cartItems);
            } else {
                console.error('Error al obtener los productos del carrito:', data.message);
                showAlert('Error al cargar los productos del carrito.', 'error');
            }
        })
        .catch((error) => {
            console.error('Error al obtener los productos del carrito:', error);
            showAlert('Error al cargar los productos del carrito.', 'error');
        });
}

function removeFromCart(productId) {
    console.log(productId);
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
                updateCartCount(data.cartItemCount, data.cartTotal);
                renderCartItems(data.cartItems);
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
    // You'll need a new API endpoint in your VentasController for this
    // e.g., POST /Ventas/DecreaseCartItem
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
                showAlert(data.message, 'success');
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



function initDropdownProvinciaCanton() {
    let provinciasDropdown = document.getElementById('provinciasDropdown');
    let cantonesDropdown = document.getElementById('cantonesDropdown');

    async function loadCantones(provinciaId, SelectedCantonId) {
        cantonesDropdown.innerHTML = '';
        cantonesDropdown.appendChild(new Option('--Cargando cantones...--', ''));
        cantonesDropdown.disabled = true;

        if (provinciaId) {
            try {
                const response = fetch(`/Usuarios/GetCantonesByProvince/${provinciaId}`);

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                const data = await response.json();
                cantonesDropdown.innerHTML = '';
                cantonesDropdown.appendChild(new Option('--Cargando cantones...--', ''));

                data.forEach((canton) => {
                    cantonesDropdown.appendChild(new Option(canton.Nombre, canton.Id));
                });
                cantonesDropdown.disabled = true;
                if (SelectedCantonId && SelectedCantonId !== 0) {
                    cantonesDropdown.value = SelectedCantonId;
                }
            } catch (error) {
                cantonesDropdown.innerHTML = '';
                cantonesDropdown.appendChild(new Option('--Error al cargar los cantones--', ''));
                cantonesDropdown.disabled = true;
            }
        } else {
            cantonesDropdown.innerHTML = '';
            cantonesDropdown.appendChild(new Option('--Seleccione una provincia--', ''));
            cantonesDropdown.disabled = true;
        }
    }

    provinciasDropdown.addEventListener('change', function () {
        let selectedProvinciaId = this.value;
        loadCantones(selectedProvinciaId)
    });
}

function showAlert(message, type = 'success') {
    const alertContainer = document.getElementById('alertContainer');
    const alert = document.createElement('div');
    alert.className = `alert alert-${type} alert-dismissible fade show`;
    alert.role = 'alert';
    alert.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;
    alertContainer.appendChild(alert);

    // Auto-cerrar la alerta después de 5 segundos
    setTimeout(() => {
        alert.classList.remove('show');
        setTimeout(() => alert.remove(), 150);
    }, 5000);
}