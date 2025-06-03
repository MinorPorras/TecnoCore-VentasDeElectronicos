document.addEventListener('DOMContentLoaded', () => {
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
});    