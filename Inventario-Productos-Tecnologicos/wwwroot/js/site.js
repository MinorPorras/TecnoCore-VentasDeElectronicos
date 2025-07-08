document.addEventListener('DOMContentLoaded', () => {
    initCartModal()

    if (document.querySelector('.register')) {
        initDropdownProvinciaCanton()
    }
});

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

function initCartModal() {
    const modalCart = document.getElementById('CarritoModal');
    const btnShowCart = document.getElementById('showCart');
    const btnCloseCart = document.getElementById('closeCart');
    const modalContent = modalCart.querySelector('.modalContent');

    function closeModalAnimation() {
        if (!modalContent) {
            return;
        }
        if (!modalContent.classList.contains('modal-fade-out')) {
            modalContent.classList.add('modal-fade-out');
            modalContent.addEventListener('animationend', () => {
                modalContent.classList.remove('modal-fade-out');
                modalCart.close();
            }, {once: true});
        }
    }

    if (modalCart && btnShowCart && btnCloseCart && modalContent) {

        btnShowCart.addEventListener('click', () => {
            modalCart.showModal();
        });

        modalCart.addEventListener('click', (e) => {
            const rect = modalContent.getBoundingClientRect();
            const isInDialog = rect.top <= e.clientY && e.clientY <= rect.top + rect.height &&
                rect.left <= e.clientX && e.clientX <= rect.left + rect.width;
            if (!isInDialog) {
                closeModalAnimation();
            }
        });

        btnCloseCart.addEventListener('click', () => {
            closeModalAnimation();
        });
    } else {
        console.log('Some elements are missing:');
        console.log('modalCart:', !!modalCart);
        console.log('btnShowCart:', !!btnShowCart);
        console.log('btnCloseCart:', !!btnCloseCart);
        console.log('modalContent:', !!modalContent);
    }
}