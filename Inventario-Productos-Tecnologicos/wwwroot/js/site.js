
document.addEventListener('DOMContentLoaded', () => {
    if (document.querySelector('.register')) {
        initDropdownProvinciaCanton()
    }
    $(document).ready(function () {
        // Inicializar Los modales globales
        initCartModal()
    });
});

function initCartModal() {
    // Asignar la animación de cierre al modal
    closeModalAnimation(modalContent, cartModal);

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
        closeModalAnimation(modalContent, cartModal)
    });
}

function initCheckoutModal() {
    closeModalAnimation(checkoutModalContent, checkoutModal);
    //Manejar el cerrar el modal del carrito al presionar clic fuera del contenido del modal
    checkoutModal.addEventListener('click', (e) => {
        const rect = checkoutModalContent.getBoundingClientRect();
        const isInDialog = rect.top <= e.clientY && e.clientY <= rect.top + rect.height &&
            rect.left <= e.clientX && e.clientX <= rect.left + rect.width;
        if (!isInDialog) {
            closeModalAnimation();
        }
    });

    // Manejar el evento de clic en el botón de cerrar del carrito
    closeCheckoutBtn.addEventListener('click', () => {
        closeModalAnimation(checkoutModalContent, checkoutModal)
    });
    
    showCheckoutBtn.addEventListener('click', () => {
        showModal(checkoutModal)
    })

    const cardNumberInput = document.getElementById('cardNumber');

    if (cardNumberInput) {
        cardNumberInput.addEventListener('input', function (e) {
            let input = e.target.value.replace(/\D/g, ''); // Eliminar todo lo que no sea dígito
            let formattedInput = '';

            // Limitar a 16 dígitos (o 19 con espacios)
            if (input.length > 16) {
                input = input.substring(0, 16);
            }

            // Añadir espacios cada 4 dígitos
            for (let i = 0; i < input.length; i++) {
                if (i > 0 && i % 4 === 0) {
                    formattedInput += ' ';
                }
                formattedInput += input[i];
            }

            e.target.value = formattedInput;
        });
    }
    
    const cardExpiryInput = document.getElementById('expirationDate');
    
    if (cardExpiryInput) {
        cardExpiryInput.addEventListener('input', function (e) {
            let input = e.target.value.replace(/\D/g, ''); // Eliminar todo lo que no sea dígito
            let formattedInput = '';

            // Limitar a 4 dígitos (MMYY)
            if (input.length > 4) {
                input = input.substring(0, 4);
            }

            // Añadir barra cada 2 dígitos
            for (let i = 0; i < input.length; i++) {
                if (i > 0 && i % 2 === 0) {
                    formattedInput += '/';
                }
                formattedInput += input[i];
            }

            e.target.value = formattedInput;
        });
    }
    
}

function closeModalAnimation(modalContent, Modal) {
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
