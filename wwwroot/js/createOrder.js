document.addEventListener('DOMContentLoaded', () => {
    const cartContainer = document.getElementById('cart-items');
    const totalElement = document.getElementById('final-sum');
    const orderForm = document.getElementById('orderForm');

    const buyerEmail = orderForm.dataset.buyerEmail;
    const userPhone = orderForm.dataset.userPhone;

    let cart = JSON.parse(localStorage.getItem('cart')) || [];

    function updateCartDisplay() {
        cartContainer.innerHTML = '';
        let totalSum = 0;

        cart.forEach((item, index) => {
            const subtotal = (item.qty * item.price).toFixed(2);
            totalSum += parseFloat(subtotal);

            const itemDiv = document.createElement('div');
            itemDiv.classList.add('cart-item');
            itemDiv.innerHTML = `
                <img src="${item.image}" alt="${item.title}">
                <div>
                    <a class="cart-item-title" href="/Shop/Albums/Details/${item.id}">${item.artist} - ${item.title}</a>
                    <div class="cart-item-controls">
                        <div class="quantity-control">
                            <button class="btn-qty minus" data-index="${index}">-</button>
                            <span class="item-qty">${item.qty}</span>
                            <button class="btn-qty plus" data-index="${index}">+</button>
                        </div>
                    </div>
                </div>
                <div class="cart-item-price">${subtotal} грн.</div>
                <button class="btn-remove" data-index="${index}">🗑️</button>
            `;
            cartContainer.appendChild(itemDiv);
        });

        totalElement.textContent = `${totalSum.toFixed(2)} грн.`;
    }

    if (cart.length > 0) {
        updateCartDisplay();
    } else {
        cartContainer.innerHTML = '<p>Ваш кошик порожній.</p>';
        totalElement.textContent = '0 грн.';
    }

    cartContainer.addEventListener('click', (e) => {
        const index = parseInt(e.target.dataset.index, 10);
        if (e.target.classList.contains('btn-qty')) {
            if (e.target.classList.contains('plus')) {
                cart[index].qty++;
            } else if (e.target.classList.contains('minus') && cart[index].qty > 1) {
                cart[index].qty--;
            }
            localStorage.setItem('cart', JSON.stringify(cart));
            updateCartDisplay();
        }

        if (e.target.classList.contains('btn-remove')) {
            cart.splice(index, 1);
            localStorage.setItem('cart', JSON.stringify(cart));
            updateCartDisplay();
        }
    });

    orderForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        if (cart.length === 0) {
            alert('Кошик порожній!');
            return;
        }

        document.getElementById('CartJsonInput').value = JSON.stringify(cart);
        if (buyerEmail) document.getElementById('BuyerEmailInput').value = buyerEmail;
        if (userPhone) document.getElementById('PhoneInput').value = userPhone;

        orderForm.submit();
        localStorage.removeItem('cart');
    });
});
