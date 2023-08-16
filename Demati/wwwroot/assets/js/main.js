//#region flagDropdown-active and languageDropdown-active
function flagDropdownAndLanguageDropdownActive() {
  const haveFlagSelected = document.querySelector("#have-flag-selected");
  const haveLanguageSelected = document.querySelector("#have-language-selected");

  haveFlagSelected.addEventListener("click", flagsDropdown)
  haveLanguageSelected.addEventListener("click", languagesDropdown);

  function flagsDropdown() {
    let flagDropdown = document.querySelector("#flagDropdown");
    const flagsDropdownIcon = document.querySelector("#flagsDropdownIcon");
    let languageDropdown = document.querySelector("#languageDropdown");
    const languageDropdownIcon = document.querySelector("#languagesDropdownIcon");

    if (flagDropdown.classList.contains("active-flag")) {
      flagDropdown.classList.remove("active-flag");
      flagsDropdownIcon.classList.remove("active-flag-icon");
    } else {
      languageDropdown.classList.remove("active-language");
      languageDropdownIcon.classList.remove("active-language-icon");
      flagDropdown.classList.add("active-flag");
      flagsDropdownIcon.classList.add("active-flag-icon");
    }
  }
  function languagesDropdown() {
    let languageDropdown = document.querySelector("#languageDropdown");
    const languageDropdownIcon = document.querySelector("#languagesDropdownIcon");

    if (languageDropdown.classList.contains("active-language")) {
      languageDropdown.classList.remove("active-language");
      languageDropdownIcon.classList.remove("active-language-icon");
    } else {
      flagDropdown.classList.remove("active-flag");
      flagsDropdownIcon.classList.remove("active-flag-icon");
      languageDropdown.classList.add("active-language");
      languageDropdownIcon.classList.add('active-language-icon');
    }
  }
}
flagDropdownAndLanguageDropdownActive();
//#endregion

//#region category-animation
function categoryHoverAnimation() {
  const parentItems = document.querySelectorAll('.categories-item');
  parentItems.forEach((parentItem) => {
    const title = parentItem.querySelector('.categories-item__title');
    const image = parentItem.querySelector('.categories-item__image');

    title.addEventListener('mouseenter', () => {
      image.style.display = 'block';
    });

    title.addEventListener('mousemove', (event) => {
      const mouseX = event.clientX - parentItem.offsetLeft;

      image.style.left = `${mouseX}px`;
    });

    title.addEventListener('mouseleave', () => {
      image.style.display = 'none';
    });
  });
}
categoryHoverAnimation();
//#endregion

//#region header-scroll
function headerScroll() {
  window.addEventListener("scroll", function () {
    let header = document.querySelector("header");
    let scrollPosition = window.scrollY || document.documentElement.scrollTop;
    if (scrollPosition > 300) {
      header.style.position = "fixed";
      header.style.width = "100%";
      // header.style.animation = 'fadeDown 0.6s ease-in-out';
      // header.style.transition = "all 0.6s ease-in-out";
    } else {
      header.style.position = "static";
      header.style.width = "auto";
      // header.style.animation = 'fadeDown 0.6s ease-in-out';
      // header.style.transition = "all 0.6s ease-in-out";
    }
  });
}
headerScroll();
//#endregion

//#region product-card=>color-active
function productCardColorActive() {
  let productCards = document.querySelectorAll(".product-card");

  function setActiveColor(event) {
    let parent = event.currentTarget.closest(".product-card");
    let colorContents = parent.querySelectorAll(".product-card__color__content");

    for (let i = 0; i < colorContents.length; i++) {
      colorContents[i].classList.remove("active-color");
    }

    event.currentTarget.classList.add("active-color");
  }

  for (let i = 0; i < productCards.length; i++) {
    let colorContents = productCards[i].querySelectorAll(".product-card__color__content");

    for (let j = 0; j < colorContents.length; j++) {
      colorContents[j].addEventListener("click", setActiveColor);
    }
  }
}
productCardColorActive();
//#endregion

//#region weekly-selection=>tablist
function weeklySelectionTablist() {
  let tabLi = document.querySelectorAll('.tab-list__li');
  let tabLiContent = document.querySelectorAll('.tab-list__content')

  tabLi.forEach((item, index) => {
    item.addEventListener('click', () => {
      tabLi.forEach(item => {
        item.classList.remove('active-selected')
      })
      item.classList.add('active-selected');

      tabLiContent.forEach(item => {
        item.classList.remove("active-content");
      })
      tabLiContent[index].classList.add('active-content');

    })
  });
}
weeklySelectionTablist();
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

//#region product-color-hover-text
function productColorHoverText() {
  const colorContentDivs = document.querySelectorAll('.product-card__color__content');

  colorContentDivs.forEach(colorContentDiv => {
    const colorBoxDiv = colorContentDiv.querySelector('.product-card__color--box');
    const hoverTextSpan = colorContentDiv.querySelector('.product-card__color--box__text');
    const hoverTextContent = colorContentDiv.querySelector('.product-card__color--box--hover-text');

    colorContentDiv.addEventListener('mouseover', () => {
      hoverTextSpan.textContent = colorBoxDiv.style.backgroundColor;
      hoverTextContent.style.display = "flex";
      hoverTextSpan.style.position = 'absolute';
    });

    colorContentDiv.addEventListener('mouseout', () => {
      hoverTextSpan.textContent = '';
      hoverTextContent.style.display = "none";
      hoverTextSpan.style.position = '';
    });
  });
}
productColorHoverText();
//#endregion

//#region product-size-active
function productSizeActive() {
  let sizeItems = document.querySelectorAll(".product-card__size__item");

  function setActiveSize(event) {
    let parent = event.currentTarget.parentNode;
    let sizeItems = parent.querySelectorAll(".product-card__size__item");

    sizeItems.forEach(item => {
      item.classList.remove("size-active");
    });

    event.currentTarget.classList.add("size-active");
  }

  sizeItems.forEach(item => {
    item.addEventListener("click", setActiveSize);
  });
}
productSizeActive();
//#endregion

//#region resetPasswordModal
function resetPasswordModal() {
  const resetBtn = document.querySelector('#resetBtn');
  const resetModal = document.querySelector('#resetPassword');
  const resetClose = document.querySelector('#resetClose');
  const loginModal = document.querySelector('#loginModal');

  resetBtn.addEventListener('click', () => {
    resetModal.style.display = 'block';
    loginModal.style.display = 'none';
  });

  resetClose.addEventListener('click', () => {
    resetModal.style.display = 'none';
    loginModal.style.display = 'block';
  });

  window.addEventListener('click', (event) => {
    if (event.target === resetModal) {
      resetModal.style.display = 'none';
    }
  });
}
resetPasswordModal();
//#endregion

//#region loginpasswordEye
function loginpasswordEye() {
  const passwordEye = document.querySelector('#passwordEye');
  const passwordInput = document.querySelector('#login-password');

  passwordEye.addEventListener('click', () => {
    if (passwordInput.type === 'password') {
      passwordInput.type = 'text';
      passwordEye.innerHTML = '<i class="fa-regular fa-eye-slash"></i>';
    } else {
      passwordInput.type = 'password';
      passwordEye.innerHTML = '<i class="fa-regular fa-eye"></i>';
    }
  });
}
loginpasswordEye();
//#endregion

//#region search-toggle
function searchToggle() {
  const searchBtn = document.querySelector('.search-toggle');
  const searchModal = document.querySelector('#search-modal');
  const closeBtn = document.querySelector('#searchClose');

  searchBtn.addEventListener('click', () => {
      searchModal.style.display = 'block';
      document.body.style.overflowY = 'hidden';
  });

  closeBtn.addEventListener('click', () => {
      searchModal.style.display = 'none';
      document.body.style.overflowY = 'scroll';
  });

  window.addEventListener('click', (event) => {
    if (event.target === searchModal) {
        searchModal.style.display = 'none';
        document.body.style.overflowY = 'scroll';
    }
  });
}
searchToggle();
//#endregion

//#region searchModal=>input
function searchModalInput() {
  const searchModalContent = document.querySelector('.search-modal-content');
  let searchModalInput = document.querySelector('#searchModalInput');
  let searchInputContent = document.querySelector('.search-modal__input');

  searchModalInput.addEventListener('click', () => {
    searchInputContent.style.borderBottom = '1px solid #000';
  })

  window.addEventListener('click', (event) => {
    if (event.target === searchModalContent) {
      console.log('asdsad');
      searchInputContent.style.borderBottom = '1px solid #ebebeb';
    }
  });
}
searchModalInput();
//#endregion

//#region basketModal
function basketModal() {
  const loginButton = document.querySelector('#header-cart');
  const basketModal = document.querySelector('#basketModal');
  const closeButton = document.querySelector('#basketClose');

  loginButton.addEventListener('click', () => {
    basketModal.style.display = 'block';
    document.body.style.overflowY = 'hidden';
  });

  closeButton.addEventListener('click', () => {
    basketModal.style.display = 'none';

    document.body.style.overflowY = 'scroll';
  });
}
basketModal();
//#endregion

window.onscroll = function() {scrollFunction()};

function scrollFunction() {
    if (document.body.scrollTop > 250 || document.documentElement.scrollTop > 250) {
        document.getElementById("scrollButton").style.display = "block";
    } else {
        document.getElementById("scrollButton").style.display = "none";
    }

    const scrollButton = document.getElementById("scrollButton");
    const maxScrollHeight = document.documentElement.scrollHeight - window.innerHeight;
    const scrollPosition = window.scrollY || document.documentElement.scrollTop || document.body.scrollTop || 0;
    const scrollPercentage = (scrollPosition / maxScrollHeight) * 100;

    scrollButton.style.backgroundImage = `linear-gradient(to right, #000 0%, #000 ${scrollPercentage}%, transparent ${scrollPercentage}%, transparent 100%)`;
  }

function scrollToTop() {
  const currentPosition = document.documentElement.scrollTop || document.body.scrollTop;
  console.log(currentPosition);
  if (currentPosition > 0) {
      window.requestAnimationFrame(scrollToTop);
      window.scrollTo(0, currentPosition - currentPosition / 10);
  }
}
