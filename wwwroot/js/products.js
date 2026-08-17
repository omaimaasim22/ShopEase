document.addEventListener("DOMContentLoaded", function () {

    const cartForms = document.querySelectorAll(".product-cart-form");

    cartForms.forEach(function (form) {

        const minusButton =
            form.querySelector(".quantity-minus");

        const plusButton =
            form.querySelector(".quantity-plus");

        const quantityNumber =
            form.querySelector(".quantity-number");

        const quantityInput =
            form.querySelector(".quantity-input");


        // Get maximum stock from the product card
        const productCard =
            form.closest(".product-card");

        const stockElement =
            productCard.querySelector(".product-stock");


        let maxStock = 999999;


        if (stockElement) {

            const stockText =
                stockElement.textContent.trim();

            const stockMatch =
                stockText.match(/\d+/);

            if (stockMatch) {
                maxStock = parseInt(stockMatch[0]);
            }
        }


        // -----------------------------------------
        // MINUS
        // -----------------------------------------

        minusButton.addEventListener("click", function () {

            let quantity =
                parseInt(quantityNumber.textContent);

            if (quantity > 1) {

                quantity--;

                quantityNumber.textContent =
                    quantity;

                quantityInput.value =
                    quantity;
            }
        });


        // -----------------------------------------
        // PLUS
        // -----------------------------------------

        plusButton.addEventListener("click", function () {

            let quantity =
                parseInt(quantityNumber.textContent);

            if (quantity < maxStock) {

                quantity++;

                quantityNumber.textContent =
                    quantity;

                quantityInput.value =
                    quantity;
            }
        });

    });

});