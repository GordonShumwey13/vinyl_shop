// Оновлення кошика
function updateCartUI() {
    const cart = JSON.parse(localStorage.getItem('cart')) || [];
    let totalQty = 0;
    let totalPrice = 0;

    cart.forEach(item => {
        totalQty += parseInt(item.qty);
        totalPrice += item.qty * item.price;
    });

    const countEl = document.querySelector('.cart-count');
    const totalEl = document.querySelector('.cart-total');

    if (countEl && totalEl) {
        countEl.textContent = totalQty;
        totalEl.textContent = `${totalPrice.toFixed(2)} грн.`;
    }
}

document.addEventListener('DOMContentLoaded', updateCartUI);

// Відкриття модального вікна авторизації
function openAuthModal() {
    const modal = document.getElementById("authModal");
    if (modal) {
        modal.style.display = "block";
    }
}

// Закриття модального вікна авторизації
function closeAuthModal() {
    const modal = document.getElementById("authModal");
    if (modal) {
        modal.style.display = "none";
    }
}

// Додавання обробника події для кнопки "Увійти"
window.addEventListener("message", function (event) {
    if (event.data?.action === "loginSuccess") {
        closeAuthModal();
        location.reload();
    }

    if (event.data?.action === "closeModal") {
        closeAuthModal();
    }
});

// Універсальна каруселька карток альбомів (.album-carousel) — може бути кілька на сторінці
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.album-carousel').forEach(carousel => {
        const track = carousel.querySelector('.album-carousel-track');
        const prevBtn = carousel.querySelector('.carousel-prev');
        const nextBtn = carousel.querySelector('.carousel-next');
        const visibleCount = 4;

        if (!track || !prevBtn || !nextBtn) {
            return;
        }

        const cards = track.querySelectorAll('.album-card');
        let currentIndex = 0;

        if (cards.length <= visibleCount) {
            prevBtn.style.display = 'none';
            nextBtn.style.display = 'none';
            return;
        }

        function updateCarousel() {
            const cardWidth = cards[0].getBoundingClientRect().width + 20;
            track.style.transform = `translateX(-${currentIndex * cardWidth}px)`;
            prevBtn.disabled = currentIndex === 0;
            nextBtn.disabled = currentIndex >= cards.length - visibleCount;
        }

        prevBtn.addEventListener('click', () => {
            if (currentIndex > 0) {
                currentIndex--;
                updateCarousel();
            }
        });

        nextBtn.addEventListener('click', () => {
            if (currentIndex < cards.length - visibleCount) {
                currentIndex++;
                updateCarousel();
            }
        });

        updateCarousel();
    });
});
