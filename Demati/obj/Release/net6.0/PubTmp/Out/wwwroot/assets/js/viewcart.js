//#region checkBoxChanged
function cartCheckboxChanged() {
    let checkbox = document.querySelector('#terms');
    const checkOutBtn = document.querySelector('.shopping-cart--checkOut-btn');
    const checkOutBtnCheck = document.querySelector('.shopping-cart--checkOut-btn--check');

    if (checkbox.checked) {
      checkOutBtn.classList.add('shopping-cart-activeBtn');
      checkOutBtnCheck.classList.add('shopping-cart-check-activeBtn');
    } else {
      checkOutBtn.classList.remove('shopping-cart-activeBtn');
      checkOutBtnCheck.classList.remove('shopping-cart-check-activeBtn');
    }
}
cartCheckboxChanged();
//#endregion

function termsAndConditionsModalCart() {
  const termsBtnViewCart = document.querySelector('#termsBtn');
  const termsModal = document.querySelector('#termsModal');
  const closeButton = document.querySelector('#termsClose');

  termsBtnViewCart.addEventListener('click', () => {
    termsModal.style.display = 'block';
  });

  closeButton.addEventListener('click', () => {
    termsModal.style.display = 'none';
  });

  window.addEventListener('click', (event) => {
    if (event.target === termsModal) {
      termsModal.style.display = 'none';
    }
  });
}
termsAndConditionsModalCart();