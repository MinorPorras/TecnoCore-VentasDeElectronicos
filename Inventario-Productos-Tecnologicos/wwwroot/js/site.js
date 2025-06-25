document.addEventListener('DOMContentLoaded', () => {
    initLoginModal();
    initCartModal()
});

function initLoginModal() {
    const modalLogin = document.getElementById('loginModal');
    const btnShowLogin = document.getElementById('showLogin');
    const btnCloseLogin = document.getElementById('closeLogin');
    const modalContent = modalLogin.querySelector('.modalContent');

    function closeModalAnimation() {
        if (!modalContent.classList.contains('modal-fade-out')) {
            modalContent.classList.add('modal-fade-out');
            modalContent.addEventListener('animationend', () => {
                modalContent.classList.remove('modal-fade-out');
                modalLogin.close();
            }, {once: true});
        }
    }

    if (modalLogin && btnShowLogin && btnCloseLogin) {
        btnShowLogin.addEventListener('click', () => {
            modalLogin.showModal();
        });
        modalLogin.addEventListener('click', (e) => {
            const rect = modalContent.getBoundingClientRect();
            const isInDialog = rect.top <= e.clientY && e.clientY <= rect.top + rect.height &&
                rect.left <= e.clientX && e.clientX <= rect.left + rect.width;
            if (!isInDialog) {
                closeModalAnimation();
            }
        })
        btnCloseLogin.addEventListener('click', closeModalAnimation);
    }
}

function initCartModal() {
    const modalCart = document.getElementById('CarritoModal');
    const btnShowCart = document.getElementById('showCart');
    const btnCloseCart = document.getElementById('closeCart');
    const modalContent = modalCart.querySelector('.modalContent');

    console.log('Modal Cart:', modalCart);
    console.log('Show Cart Button:', btnShowCart);
    console.log('Close Cart Button:', btnCloseCart);
    console.log('Modal Content:', modalContent);

    function closeModalAnimation() {
        console.log('Closing modal animation triggered');
        if (!modalContent) {
            console.error('Modal content not found');
            return;
        }
        if (!modalContent.classList.contains('modal-fade-out')) {
            console.log('Entering fade-out animation');
            modalContent.classList.add('modal-fade-out');
            modalContent.addEventListener('animationend', () => {
                console.log('Animation ended, closing modal');
                modalContent.classList.remove('modal-fade-out');
                modalCart.close();
            }, {once: true});
        }
    }

    if (modalCart && btnShowCart && btnCloseCart && modalContent) {
        console.log('All elements found, setting up event listeners');

        btnShowCart.addEventListener('click', () => {
            console.log('Show cart clicked');
            modalCart.showModal();
        });

        modalCart.addEventListener('click', (e) => {
            const rect = modalContent.getBoundingClientRect();
            const isInDialog = rect.top <= e.clientY && e.clientY <= rect.top + rect.height &&
                rect.left <= e.clientX && e.clientX <= rect.left + rect.width;
            if (!isInDialog) {
                console.log('Clicked outside modal');
                closeModalAnimation();
            }
        });

        btnCloseCart.addEventListener('click', () => {
            console.log('Close cart clicked');
            closeModalAnimation();
        });
    } else {
        console.log('Some elements are missing:');
        console.log('modalCart:', !!modalCart);
        console.log('btnShowCart:', !!btnShowCart);
        console.log('btnCloseCart:', !!btnCloseCart);
        console.log('modalContent:', !!modalContent);
    }

    if (localStorage.getItem("UserId") && localStorage.getItem("UserId") !== "null") {
        fetch(`/Ventas/GetCartItems`)
            .then(response => response.json()
                .then(data => {
                    const cartItemsContainer = document.getElementById('cart-item-container');
                    if (cartItemsContainer) {
                        cartItemsContainer.innerHTML = '';
                        if (data.length > 0) {
                            console.log("Datos del carrito:", data);
                            data.forEach(item => {
                                const itemElement = document.createElement('div');
                                itemElement.className = 'cart-item';
                                itemElement.innerHTML = `
                                <span class="item-code">ID: ${item.ProductoId}</span>
                                <span class="item-name">${item.Producto.Nombre}</span>
                                <span class="item-quantity">×${item.Cantidad}</span>
                                <span class="item-price">₡${item.Producto.Precio.toFixed(2)}</span>
                                <button class="remove-item" data-id="${item.ProductoId}">×</button>`;
                                cartItemsContainer.appendChild(itemElement);
                            });
                        } else {
                            cartItemsContainer.innerHTML = '<p>No hay productos en el carrito.</p>';
                        }

                        // Actualizar el total
                        const total = data.reduce((sum, item) => sum + (item.Producto.Precio * item.Cantidad), 0);
                        const totalElement = document.querySelector('#CarritoModal .modalContent > p:last-of-type');
                        if (totalElement) {
                            totalElement.textContent = `Total: ₡${total.toFixed(2)}`;
                        }

                        const removeButtons = document.querySelectorAll('.remove-item');
                        removeButtons.forEach(button => button.addEventListener('click', (e) => {
                            const productId = e.target.getAttribute('data-id');
                            fetch(`/Ventas/RemoveFromCart/${productId}`, {
                                method: 'POST'
                            }).then(response => response.json().then(data => {
                                if (data.success) {
                                    e.target.closest('.cart-item').remove();
                                    if (cartItemsContainer.children.length === 0) {
                                        cartItemsContainer.innerHTML = '<p>No hay productos en el carrito.</p>';
                                    }
                                } else {
                                    alert('Error al eliminar el producto del carrito.');
                                }
                            }));
                        }));
                    }
                }));
    } else {
        const cartItemsContainer = document.getElementById('cart-item-container');
        if (cartItemsContainer) {
            cartItemsContainer.innerHTML = '<p>No hay productos en el carrito.</p>';
        }
    }
}