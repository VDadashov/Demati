//#region imageThumbnails
function imageThumbnails() {
    let productImages = document.querySelectorAll('.product-item__image');
    let mainImages = document.querySelectorAll('.product-item__main-image-item');

    productImages.forEach(image => {
        image.addEventListener('click', function () {
            productImages.forEach(function (img) {
                img.classList.remove('active-s');
            });

            image.classList.add('active-s');
        });
    });

    productImages.forEach((image, index) => {
        image.addEventListener('click', function () {
            mainImages.forEach((element, indexer) => {
                if (index == indexer) {
                    mainImages.forEach(img => {
                        img.classList.remove('active-m');
                    })
                    element.classList.add('active-m')
                }
            });
        })
    });
}
imageThumbnails();
//#endregion

//#region sizeActive
function sizeActive() {
    let boxSize = document.querySelectorAll('.product-item__size__box');
    let sizeValue = document.querySelector('.product-item__size__value');

    boxSize.forEach(item => {
        item.addEventListener('click', function () {
            boxSize.forEach(item => {
                item.classList.remove('active-size');
            })
            let span = item.querySelector('span');
            sizeValue.textContent = span.textContent;
            item.classList.add('active-size');
        })
        boxSize.forEach(item => {
            if(item.classList.contains('active-size')){
              let span = item.querySelector('span');
              sizeValue.textContent = span.textContent;
            }
        })
    });
}
sizeActive();
//#endregion

//#region colorHoverSize
function colorHoverSize() {
    const colorContentDivs = document.querySelectorAll('.product-item__color__box');

    colorContentDivs.forEach(colorContentDiv => {
        const colorBoxDiv = colorContentDiv.querySelector('.product-item__color--box');
        const hoverTextSpan = colorContentDiv.querySelector('.product-item__color--box__text');
        const hoverTextContent = colorContentDiv.querySelector('.product-item__color--box--hover-text');

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
colorHoverSize();
//#endregion

//#region countQty
function countQty() {
    document.querySelector('.product-item__qty-minus').addEventListener('click', function () {
        var inputElement = document.getElementById('product-item--count');
        var value = parseInt(inputElement.value);

        if (value > 1) {
            inputElement.value = value - 1;
        }
    });

    document.querySelector('.product-item__qty-plus').addEventListener('click', function () {
        var inputElement = document.getElementById('product-item--count');
        var value = parseInt(inputElement.value);

        inputElement.value = value + 1;
    });
}
countQty();
//#endregion

//#region wishlistBtnActive
function wishlistBtnActive() {
    let wishlistBtn = document.querySelector(".product-item--wishlist");

    wishlistBtn.addEventListener('click', function () {
        if (wishlistBtn.classList.contains('active')) {
            wishlistBtn.classList.remove('active');
        } else {
            wishlistBtn.classList.add('active');
        }
    })
}
wishlistBtnActive();
//#endregion

//#region sizeGuideModal
function sizeGuideModal() {
    let sizeGuide = document.getElementById("sizeGuide");
    let modal = document.getElementById("size-modal");
    let close = document.getElementsByClassName("close")[0];

    sizeGuide.addEventListener("click", function () {
        modal.style.display = "block";
    });

    close.addEventListener("click", function () {
        modal.style.display = "none";
    });

    window.addEventListener("click", function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    });
}
sizeGuideModal();
//#endregion

//#region askQuestionModal
function askQuestionModal() {
    let askQuestionButton = document.getElementById("askQuestionBtn");
    let modal = document.getElementById("askQuestionModal");
    let close = document.getElementsByClassName("askQuestionModal-close")[0];

    askQuestionButton.addEventListener("click", function () {
        modal.style.display = "block";
    });

    close.addEventListener("click", function () {
        modal.style.display = "none";
    });

    window.addEventListener("click", function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    });
}
askQuestionModal();
//#endregion

//#region deliveryModal
function deliveryModal() {
    let askQuestionButton = document.getElementById("deliveryBtn");
    let modal = document.getElementById("deliveryModal");
    let close = document.getElementsByClassName("deliveryModal-close")[0];

    askQuestionButton.addEventListener("click", function () {
        modal.style.display = "block";
    });

    close.addEventListener("click", function () {
        modal.style.display = "none";
    });

    window.addEventListener("click", function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    });
}
deliveryModal();
//#endregion

//#region shareModal
function shareModal() {
    let askQuestionButton = document.getElementById("shareBtn");
    let modal = document.getElementById("shareModal");
    let close = document.getElementsByClassName("shareModal-close")[0];
    let urlInput = document.querySelector('#shareModal-input');

    askQuestionButton.addEventListener("click", function () {
        modal.style.display = "block";
        let url = window.location.href;
        urlInput.value = url;
    });

    close.addEventListener("click", function () {
        modal.style.display = "none";
    });

    window.addEventListener("click", function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    });
}
shareModal();
//#endregion

//#region copyUrl
function copyUrl() {
    let copyBtn = document.querySelector('#shareModal-copy');
    let urlInput = document.querySelector('#shareModal-input');

    copyBtn.addEventListener('click', function () {
        urlInput.select();
        urlInput.setSelectionRange(0, 99999);

        navigator.clipboard.writeText(urlInput.value);

        alert("Copied the text: " + urlInput.value);
    })
}
copyUrl();
//#endregion

//#region colorActive
function colorActive() {
    let colors = document.querySelectorAll('.product-item__color__box');
    let colorValue = document.querySelector('.product-item__color__value');

    colors.forEach(color => {
        color.addEventListener('click', function () {
            colors.forEach(item => {
                item.classList.remove('active-color');
            })
            colorName = color.querySelector('.product-item__color--box').style.backgroundColor;
            colorValue.textContent = colorName;
            color.classList.add('active-color');
        })
        colors.forEach(item => {
            if (item.classList.contains('active-color')) {
                colorName = item.querySelector('.product-item__color--box').style.backgroundColor;
                colorValue.textContent = colorName;
            }
        })
    })
}
colorActive();
//#endregion

//#region modalImageActive
function modalImageActive() {
    let mainImages = document.querySelectorAll('.product-item__main-image-item');
    let modal = document.getElementById('imageModal');
    let modalImage = document.querySelector('.modal-image');
    let close = document.querySelector('.imageModal-close');

    mainImages.forEach(mainImage => {
        mainImage.addEventListener('click', function () {
            if (mainImage.classList.contains('active-m')) {
                modal.style.display = 'block';
                image = mainImage.querySelector('.product-item__main-image');
                modalImage.src = image.src;
            }
        });
    })

    close.addEventListener('click', function () {
        modal.style.display = 'none';
    });
}
modalImageActive();
//#endregion

//#region productTabList
function productTabList() {
    let tabLi = document.querySelectorAll('.product-tab-list__li');
    let tabLiContent = document.querySelectorAll('.product-tab-list__content')

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
productTabList();
//#endregion

//#region reviewCommentBtn
function reviewCommentBtn() {
    let commentBtn = document.querySelector('#reviewBtn');
    let commentContent = document.querySelector('.review__write-content');

    commentBtn.addEventListener('click', function () {
        if (commentContent.classList.contains('active-review')) {
            commentContent.classList.remove('active-review');
            commentContent.style.display = "none";
        } else {
            commentContent.classList.add('active-review');
            commentContent.style.display = "block";
        }
    })
}
reviewCommentBtn();
//#endregion

//#region reviewCharacterLimit
function reviewCharacterLimit() {
    const reviewDescription = document.getElementById("review-description");
    const charCount = document.getElementById("charCount");

    reviewDescription.addEventListener("input", updateCharCount);

    function updateCharCount() {
        const maxLength = reviewDescription.getAttribute("maxlength");
        const currentLength = reviewDescription.value.length;
        const remainingChars = maxLength - currentLength;

        charCount.textContent = "Body of Review (" + remainingChars + ")";
    }
}
reviewCharacterLimit();
//#endregion

const starLabels = document.querySelectorAll('.write-content__choose-raiting--star');

starLabels.forEach(label => {
    label.addEventListener('click', function () {
        const clickedIndex = [...starLabels].indexOf(this);
        starLabels.forEach((starLabel, index) => {
            if (index <= clickedIndex) {
                starLabel.querySelector('i').classList.add('active-star');
            } else {
                starLabel.querySelector('i').classList.remove('active-star');
            }
        });
    });

    label.addEventListener('mouseenter', function () {
        const hoveredIndex = [...starLabels].indexOf(this);
        starLabels.forEach((starLabel, index) => {
            if (index <= hoveredIndex) {
                starLabel.querySelector('i').classList.add('active-star-hover');
            } else {
                starLabel.querySelector('i').classList.remove('active-star-hover');
            }
        });
    });

    label.addEventListener('mouseleave', function () {
        starLabels.forEach(starLabel => {
            starLabel.querySelector('i').classList.remove('active-star-hover');
        });
    });
});




