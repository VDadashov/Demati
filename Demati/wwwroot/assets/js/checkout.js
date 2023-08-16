//#region delivery
function radioChanged(radioInput) {
  const radioInputs = document.querySelectorAll('.checkout__information__radio');
  const deliveryDivs = document.querySelectorAll('.delivery-radio');
  const shippingAddress = document.querySelector('.shipping-address');
  const pickupLocation = document.querySelector('.pickup-locations');
  const shippingPrice = document.querySelector('.checkout__products__shipping');
  const pickupPrice = document.querySelector('.checkout__products__pick-up');
  const shippingAddressBreadCrumb = document.querySelector('.shipping-address-breadcrumb');

  for (let i = 0; i < radioInputs.length; i++) {
    if (radioInputs[i].checked) {
      if(radioInputs[i].id === "information-ship"){
        shippingAddress.style.display = 'block';
        shippingPrice.style.display = 'flex';
        shippingAddressBreadCrumb.style.display = 'block';
        pickupLocation.style.display = 'none';
        pickupPrice.style.display = 'none';
      }else{
        shippingAddress.style.display = 'none';
        shippingPrice.style.display = 'none';
        shippingAddressBreadCrumb.style.display = 'none';
        pickupLocation.style.display = 'block';
        pickupPrice.style.display = 'flex';
      }
      deliveryDivs[i].style.border = '1px solid #31abbe';
      deliveryDivs[i].style.backgroundColor = 'rgba(54, 84, 175, 0.08)'
    } else {
      deliveryDivs[i].style.border = '';
      deliveryDivs[i].style.backgroundColor = '#fff'
    }
  }
}

radioChanged();

//#endregion

//#region fname=>Placeholder
function fnamePlaceholder() {
  const inputFirstName = document.getElementById("shipping-address__fname");
  const firstNamePlaceholder = document.querySelector(".placeholder-fname");
  firstNamePlaceholder.textContent = inputFirstName.getAttribute("placeholder");

  inputFirstName.addEventListener("keyup", function () {
    if (inputFirstName.value !== "") {
      firstNamePlaceholder.style.display = "inline";
      inputFirstName.style.padding = "20.5px 30px 5.5px 11px";
    } else {
      firstNamePlaceholder.style.display = "none";
      inputFirstName.style.animation = 'none';
      inputFirstName.style.padding = "13.5px 11px 13.5px 11px";
    }
  });
}
fnamePlaceholder();
//#endregion

//#region lname=>Placeholder
function lnamePlaceholder() {
  const inputLastName = document.getElementById("shipping-address__lname");
  const lastNamePlaceholder = document.querySelector(".placeholder-lname");
  lastNamePlaceholder.textContent = inputLastName.getAttribute("placeholder");

  inputLastName.addEventListener("keyup", function () {
    if (inputLastName.value !== "") {
      lastNamePlaceholder.style.display = "inline";
      inputLastName.style.padding = "20.5px 30px 5.5px 11px";
    } else {
      lastNamePlaceholder.style.display = "none";
      inputLastName.style.animation = 'none'
      inputLastName.style.padding = "13.5px 11px 13.5px 11px";
    }
  });
}
lnamePlaceholder();
//#endregion

//#region address=>Placeholder
function addressPlaceholder() {
  const inputAddress = document.getElementById("shipping-address__address");
  const addressPlaceholder = document.querySelector(".placeholder-address");
  addressPlaceholder.textContent = inputAddress.getAttribute("placeholder");

  inputAddress.addEventListener("keyup", function () {
    if (inputAddress.value !== "") {
      addressPlaceholder.style.display = "inline";
      inputAddress.style.padding = "20.5px 30px 5.5px 11px";
    } else {
      addressPlaceholder.style.display = "none";
      inputAddress.style.animation = "none";
      inputAddress.style.padding = "13.5px 11px 13.5px 11px";
    }
  });
}
addressPlaceholder();
//#endregion

//#region apartment=>Placeholder
function apartmentPlaceholder() {
  const inputApartment = document.getElementById("shipping-address__apartment");
  const apartmentPlaceholder = document.querySelector(".placeholder-apartment");
  apartmentPlaceholder.textContent = inputApartment.getAttribute("placeholder");

  inputApartment.addEventListener("keyup", function () {
    if (inputApartment.value !== "") {
      apartmentPlaceholder.style.display = "inline";
      inputApartment.style.padding = "20.5px 30px 5.5px 11px";
    } else {
      apartmentPlaceholder.style.display = "none";
      inputApartment.style.animation = 'none';
      inputApartment.style.padding = "13.5px 11px 13.5px 11px";
    }
  });
}
apartmentPlaceholder();
//#endregion

//#region phone=>Placeholder
function phonePlaceholder() {
    const inputApartment = document.getElementById("shipping-address__phone");
    const apartmentPlaceholder = document.querySelector(".placeholder-phone");
    apartmentPlaceholder.textContent = inputApartment.getAttribute("placeholder");

    inputApartment.addEventListener("keyup", function () {
        if (inputApartment.value !== "") {
            apartmentPlaceholder.style.display = "inline";
            inputApartment.style.padding = "20.5px 30px 5.5px 11px";
        } else {
            apartmentPlaceholder.style.display = "none";
            inputApartment.style.animation = 'none';
            inputApartment.style.padding = "13.5px 11px 13.5px 11px";
        }
    });
}
phonePlaceholder();
//#endregion

//#region postalCode=>Placeholder
function postalCodePlaceholder() {
  const inputPostalCode = document.getElementById("shipping-address__postal-code");
  const postalCodePlaceholder = document.querySelector(".placeholder-postal-code");
  postalCodePlaceholder.textContent = inputPostalCode.getAttribute("placeholder");

  inputPostalCode.addEventListener("keyup", function () {
    if (inputPostalCode.value !== "") {
      postalCodePlaceholder.style.display = "inline";
      inputPostalCode.style.padding = "20.5px 30px 5.5px 11px";
    } else {
      postalCodePlaceholder.style.display = "none";
      inputPostalCode.style.animation = 'none';
      inputPostalCode.style.padding = "13.5px 11px 13.5px 11px";
    }
  });
}
postalCodePlaceholder();
//#endregion

//#region city=>Placeholder
function cityPlaceholder() {
  const inputCity = document.getElementById("shipping-address__city");
  const cityPlaceholder = document.querySelector(".placeholder-city");
  cityPlaceholder.textContent = inputCity.getAttribute("placeholder");

  inputCity.addEventListener("keyup", function () {
    if (inputCity.value !== "") {
      cityPlaceholder.style.display = "inline";
      inputCity.style.padding = "20.5px 30px 5.5px 11px";
    } else {
      cityPlaceholder.style.display = "none";
      inputCity.style.animation = 'none';
      inputCity.style.padding = "13.5px 11px 13.5px 11px";
    }
  });
}
cityPlaceholder();
//#endregion

//#region load
function load() {
    window.addEventListener("load", function () {
        let loadingScreen = document.getElementById("loading-screen");
        loadingScreen.style.display = "none";
    });
}
load();
//#endregion

function giftCardPlaceholder() {
  const inputGIftCard = document.getElementById("checkout-gift-card");
  const giftCardPlaceholder = document.querySelector(".placeholder-gift-card");
  const giftCardBtn = document.querySelector('.checkout-gift-card-btn');
  giftCardPlaceholder.textContent = inputGIftCard.getAttribute("placeholder");

  inputGIftCard.addEventListener("keyup", function () {
    if (inputGIftCard.value !== "") {
      giftCardPlaceholder.style.display = "inline";
      inputGIftCard.style.padding = "20.5px 30px 5.5px 11px";
      giftCardBtn.style.backgroundColor = '#458CBB';
      giftCardBtn.disabled = false;
      giftCardBtn.style.cursor = 'pointer';
    } else {
      giftCardPlaceholder.style.display = "none";
      inputGIftCard.style.animation = 'none'
      giftCardBtn.style.backgroundColor = 'rgba(110, 110, 110,0.4)';
      inputGIftCard.style.padding = "13.5px 11px 13.5px 11px";
      giftCardBtn.disabled = true;
      giftCardBtn.style.cursor = 'default';
    }
  });
}
giftCardPlaceholder();
