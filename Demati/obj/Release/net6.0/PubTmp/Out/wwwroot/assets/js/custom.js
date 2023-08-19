$(document).ready(function () {
    let page = window.location.pathname.split('/')[1];

    if (page.length == 0) {
        $('.Home').addClass('active-menu');
    } else {
        $('.' + page).addClass('active-menu');
    }

    //search
    $('#searchModalInput').keyup(function (e) {
        let search = $(this).val();
        if (search.length > 0) {
            fetch('/product/search/?search=' + search)
                .then(res => {
                    return res.text();
                }).then(data => {
                    $('.search-result').html(data);
                })
        } else {
            $('.search-result').html("")
        }
    })

    //$('#searchBtn').click(function (e) {
    //    let search = $('#searchModalInput').val();
    //    if (search.length > 0) {
    //        fetch("/shop/inedx?searchValue=" + search)
    //            .then(res => {
    //                return res.text();
    //            })
    //            .then(data => {
    //                $('.shop').html(data);
    //            });
    //    }
    //})

    //modal

    $('.modalBtn').click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href');

        fetch(url)
            .then(res => res.text())
            .then(data => {
                $('.productModal-content').html(data);

                //#region productModal
                function productModal() {
                    const productModal = document.querySelector('#productModal');
                    const productModalClose = document.querySelector('#productModalClose');

                    productModalClose.addEventListener('click', () => {
                        productModal.style.display = 'none';
                    });

                    window.addEventListener('click', (event) => {
                        if (event.target === productModal) {
                            productModal.style.display = 'none';
                        }
                    });
                }
                function openModal() {
                    const productModal = document.querySelector('#productModal');
                    productModal.style.display = "block";
                }
                productModal();
                //#endregion

                //#region productModalColorHoverSize
                function modalColorHoverSize() {
                    const colorContentDivs = document.querySelectorAll('.productModal__color__box');

                    colorContentDivs.forEach(colorContentDiv => {
                        const colorBoxDiv = colorContentDiv.querySelector('.productModal__color--box');
                        const hoverTextSpan = colorContentDiv.querySelector('.productModal__color--box__text');
                        const hoverTextContent = colorContentDiv.querySelector('.productModal__color--box--hover-text');

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
                modalColorHoverSize();
                //#endregion

                //#region productModalColorActive
                function modalColorActive() {
                    let colors = document.querySelectorAll('.productModal__color__box');
                    let colorValue = document.querySelector('.productModal__color__value');

                    colors.forEach(color => {
                        color.addEventListener('click', function () {
                            colors.forEach(item => {
                                item.classList.remove('active-color');
                            })
                            colorName = color.querySelector('.productModal__color--box').style.backgroundColor;
                            colorValue.textContent = colorName;
                            color.classList.add('active-color');
                        })
                        colors.forEach(item => {
                            if (item.classList.contains('active-color')) {
                                colorName = item.querySelector('.productModal__color--box').style.backgroundColor;
                                colorValue.textContent = colorName;
                            }
                        })
                    })
                }
                modalColorActive();
                //#endregion

                //#region productModalSizeActive
                function modalSizeActive() {
                    let boxSize = document.querySelectorAll('.productModal__size__box');
                    let sizeValue = document.querySelector('.productModal__size__value');

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
                            if (item.classList.contains('active-size')) {
                                let span = item.querySelector('span');
                                sizeValue.textContent = span.textContent;
                            }
                        })
                    });
                }
                modalSizeActive();
                //#endregion

                //#region productModalcountQty
                function productModalcountQty() {
                    document.querySelector('.productModal__qty-minus').addEventListener('click', function () {
                        let inputElement = document.getElementById('productModal--count');
                        let value = parseInt(inputElement.value);

                        if (value > 1) {
                            inputElement.value = value - 1;
                        }
                    });

                    document.querySelector('.productModal__qty-plus').addEventListener('click', function () {
                        let inputElement = document.getElementById('productModal--count');
                        let value = parseInt(inputElement.value);

                        inputElement.value = value + 1;
                    });
                }
                productModalcountQty();
                //#endregion

                //#region productModal
                function productModal() {
                    const productModal = document.querySelector('#productModal');
                    const productModalClose = document.querySelector('#productModalClose');

                    productModalClose.addEventListener('click', () => {
                        productModal.style.display = 'none';
                    });

                    window.addEventListener('click', (event) => {
                        if (event.target === productModal) {
                            productModal.style.display = 'none';
                        }
                    });
                }
                productModal();
                //#endregion

                //#region productModal-imageThumbnail
                $('.productModal__imageThumbnail').slick({
                    dots: true,
                    infinite: true,
                    speed: 400,
                    fade: true,
                    cssEase: 'linear'
                });
                    //#endregion
            });
    })

    //shop-pagination
    $(document).on('click', '.pageIn', function () {
        let categoryId = $(this).attr('data-categoryId');
        let pageIndex = $(this).attr('data-pageIndex');
        let sizeId = $(this).attr('data-sizeId');
        let availability = $(this).attr('data-availability');
        let selectId = $(this).attr('data-selectId');
        let colorId = $(this).attr('data-colorId');
        let minPrice = $(".min-input").val();
        let maxPrice = $(".max-input").val();

        fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId=" + sizeId
            + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shop').html(data);
            });
    });

    //shop-category
    $(document).on('click', '.shop__filter__categories__link', function () {

        let sizeId = $(this).attr('data-sizeId');
        let categoryId = $(this).attr('data-categoryId');
        let colorId = $(this).attr('data-colorId');
        let selectId = $(this).attr('data-selectId');
        let pageIndex = $(".pageIn").attr('data-pageIndex');
        let availability = $(this).attr('data-availability');
        let minPrice = $(".min-input").val();
        let maxPrice = $(".max-input").val();

        fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId
            + "&sizeId=" + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shop').html(data);
            });
    });

    //shop-aviabilty
    $(document).on('click', '.shop__filter__availability__link', function (e) {
        e.preventDefault();

        let categoryId = $(this).attr('data-categoryId');
        let colorId = $(this).attr('data-colorId');
        let sizeId = $(this).attr('data-sizeId');
        let pageIndex = $(".pageIn").attr('data-pageIndex');
        let availability = $(this).attr('data-availability');
        let selectId = $(this).attr('data-selectId');
        let minPrice = $(".min-input").val();
        let maxPrice = $(".max-input").val();

        fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId="
            + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shop').html(data);
            });
    });

    let delay = 700;
    let timeoutId;

    //shop-price-min-range
    $(document).on('input', '.min-range', function () {

        clearTimeout(timeoutId);

        timeoutId = setTimeout(function () {
            let colorId = $(".progress").attr('data-colorId');
            let sizeId = $(".progress").attr('data-sizeId');
            let categoryId = $(".progress").attr('data-categoryId');
            let availability = $(".progress").attr('data-availability');
            let selectId = $(".progress").attr('data-selectId');
            let minPrice = $(".min-input").val();
            let maxPrice = $(".max-input").val();
            let pageIndex = $(".pageIn").attr('data-pageIndex');

            fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId="
                + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
                .then(res => {
                    return res.text();
                })
                .then(data => {
                    $('.shop').html(data);
                });
        }, delay);
    });

    //shop-price-max-range
    $(document).on('input', '.max-range', function () {

        clearTimeout(timeoutId);

        timeoutId = setTimeout(function () {
            let colorId = $(".progress").attr('data-colorId');
            let sizeId = $(".progress").attr('data-sizeId');
            let categoryId = $(".progress").attr('data-categoryId');
            let availability = $(".progress").attr('data-availability');
            let selectId = $(".progress").attr('data-selectId');
            let minPrice = $(".min-input").val();
            let maxPrice = $(".max-input").val();
            let pageIndex = $(".pageIn").attr('data-pageIndex');

            fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId="
                + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
                .then(res => {
                    return res.text();
                })
                .then(data => {
                    $('.shop').html(data);
                });
        }, delay);
    });

    //shop-price-min-input
    $(document).on('input', '.min-input', function () {

        clearTimeout(timeoutId);

        timeoutId = setTimeout(function () {
            let colorId = $(".progress").attr('data-colorId');
            let sizeId = $(".progress").attr('data-sizeId');
            let categoryId = $(".progress").attr('data-categoryId');
            let availability = $(".progress").attr('data-availability');
            let selectId = $(".progress").attr('data-selectId');
            let minPrice = $(".min-input").val();
            let maxPrice = $(".max-input").val();
            let pageIndex = $(".pageIn").attr('data-pageIndex');

            fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId="
                + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
                .then(res => {
                    return res.text();
                })
                .then(data => {
                    $('.shop').html(data);
                });
        }, delay);
    });

    //shop-price-max-input
    $(document).on('input', '.max-input', function () {

        clearTimeout(timeoutId);

        timeoutId = setTimeout(function () {
            let colorId = $(".progress").attr('data-colorId');
            let sizeId = $(".progress").attr('data-sizeId');
            let categoryId = $(".progress").attr('data-categoryId');
            let availability = $(".progress").attr('data-availability');
            let selectId = $(".progress").attr('data-selectId');
            let minPrice = $(".min-input").val();
            let maxPrice = $(".max-input").val();
            let pageIndex = $(".pageIn").attr('data-pageIndex');

            fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId="
                + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
                .then(res => {
                    return res.text();
                })
                .then(data => {
                    $('.shop').html(data);
                });
        }, delay);
    });

    //shop-color
    $(document).on('click', '.shop__filter__color__content', function () {

        let colorId = $(this).attr('data-colorId');
        let sizeId = $(this).attr('data-sizeId');
        let categoryId = $(this).attr('data-categoryId');
        let availability = $(this).attr('data-availability');
        let selectId = $(this).attr('data-selectId');
        let pageIndex = $(".pageIn").attr('data-pageIndex');
        let minPrice = $(".min-input").val();
        let maxPrice = $(".max-input").val();

        fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId="
            + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shop').html(data);
            });
    });

    //shop-size
    $(document).on('click', '.shop__filter__size__item', function () {

        let sizeId = $(this).attr('data-sizeId');
        let colorId = $(this).attr('data-colorId');
        let categoryId = $(this).attr('data-categoryId');
        let availability = $(this).attr('data-availability');
        let selectId = $(this).attr('data-selectId');
        let pageIndex = $(".pageIn").attr('data-pageIndex');
        let minPrice = $(".min-input").val();
        let maxPrice = $(".max-input").val();

        fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId="
            + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shop').html(data);
            });
    });

    //shop-filter-reset-btn
    $(document).on('click', '.filter-reset-btn', function () {

        let sizeId = $(this).attr('data-sizeId');
        let colorId = $(this).attr('data-colorId');
        let categoryId = $(this).attr('data-categoryId');
        let availability = $(this).attr('data-availability');
        let selectId = $(this).attr('data-selectId');
        let pageIndex = $(".pageIn").attr('data-pageIndex');
        let minPrice = $(".min-input").val();
        let maxPrice = $(".max-input").val();
        let filterChecked = false;

        fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId="
            + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&filterChecked=" + filterChecked + "&selectId=" + selectId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shop').html(data);
            });
    });

    //shop-select
    $(document).on("change", "#productFilter",function () {
        var selectedOption = $(this).find("option:selected");

        var selectId = selectedOption.attr("data-selectId");

        let colorId = selectedOption.attr('data-colorId');
        let sizeId = selectedOption.attr('data-sizeId');
        let categoryId = selectedOption.attr('data-categoryId');
        let availability = selectedOption.attr('data-availability');
        let pageIndex = $(".pageIn").attr('data-pageIndex');
        let minPrice = $(".min-input").val();
        let maxPrice = $(".max-input").val();

        fetch("/shop/shopfilters?colorId=" + colorId + "&categoryId=" + categoryId + "&sizeId="
            + sizeId + "&pageIndex=" + pageIndex + "&availability=" + availability + "&minPrice=" + minPrice + "&maxPrice=" + maxPrice + "&selectId=" + selectId)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shop').html(data);
            });
    });

    //addBasket
    $(document).on('click', '.basketBtn', function (e) {
        e.preventDefault();

        let id = $(this).attr('data-Id');
        let colorId;
        let sizeId;

        var sizeItems = $(this).parent().parent().parent().find(".product-card__size__item");
        var activeSizeItem = sizeItems.filter(".size-active");
        sizeId = activeSizeItem.attr("data-Id");

        var colorItems = $(this).parent().parent().parent().find(".product-card__color__content");
        var activeColorItem = colorItems.filter(".active-color");
        colorId = activeColorItem.attr("data-Id");

        fetch("/Basket/AddBasket?Id=" + id + "&colorId=" + colorId + "&sizeId=" + sizeId)
            .then(res => res.text())
            .then(data => {
                $('.basketModal-content').html(data)

                fetch("/Basket/RefreshBasketCount")
                    .then(res2 => res2.text())
                    .then(data2 => {
                        $('.header-cart__count').html(data2);
                    })
            })

        $('.addBasketAlert').addClass('show');

        setTimeout(function () {
            $('.addBasketAlert').removeClass('show');
        }, 1000);
    })

    //addBasket-detail
    $(document).on('click', '.product-item__add-to-card__btn', function (e) {
        e.preventDefault();

        let id = $(this).attr('data-Id');
        let count = $("#product-item--count").val();
        let colorId;
        let sizeId;

        var sizeItems = $(this).parent().parent().find(".product-item__size__box");
        var activeSizeItem = sizeItems.filter(".active-size");
        sizeId = activeSizeItem.attr("data-sizeId");

        var colorItems = $(this).parent().parent().parent().find(".product-item__color__box");
        var activeColorItem = colorItems.filter(".active-color");
        colorId = activeColorItem.attr("data-colorId");

        fetch("/Product/AddBasketCount?Id=" + id + "&colorId=" + colorId + "&sizeId=" + sizeId + "&count=" + count)
            .then(res => res.text())
            .then(data => {
                $('.basketModal-content').html(data)


                fetch("/Basket/RefreshBasketCount")
                    .then(res2 => res2.text())
                    .then(data2 => {
                        $('.header-cart__count').html(data2);
                    })
            })


        $('.addBasketAlert').addClass('show');

        setTimeout(function () {
            $('.addBasketAlert').removeClass('show');
        }, 1000);
    })

    //addBasket-modal
    $(document).on('click', '.productModal__add-to-card__btn', function (e) {
        e.preventDefault();

        let id = $(this).attr('data-Id');
        let count = $("#productModal--count").val();
        let colorId;
        let sizeId;

        var sizeItems = $(this).parent().parent().find(".productModal__size__box");
        var activeSizeItem = sizeItems.filter(".active-size");
        sizeId = activeSizeItem.attr("data-sizeId");

        var colorItems = $(this).parent().parent().parent().find(".productModal__color__box");
        var activeColorItem = colorItems.filter(".active-color");
        colorId = activeColorItem.attr("data-colorId");

        fetch("/Product/AddBasketCount?Id=" + id + "&colorId=" + colorId + "&sizeId=" + sizeId + "&count=" + count)
            .then(res => res.text())
            .then(data => {
                $('.basketModal-content').html(data)

                fetch("/Basket/RefreshBasketCount")
                    .then(res2 => res2.text())
                    .then(data2 => {
                        $('.header-cart__count').html(data2);
                    })
            })


        $('.addBasketAlert').addClass('show');

        setTimeout(function () {
            $('.addBasketAlert').removeClass('show');
        }, 1000);
    })

    //deleteBasket
    $(document).on('click', '.basketModal__item--remove', function (e) {
        e.preventDefault();

        let id = $(this).attr('data-Id');
        let count = $(this).attr('data-count');
        let colorId = $(this).attr('data-colorId');
        let sizeId = $(this).attr('data-sizeId');

        fetch("/Basket/DeletefromBasket?Id=" + id + "&colorId=" + colorId + "&sizeId=" + sizeId + "&count=" + count)
            .then(res => res.text())
            .then(data => {
                $('.basketModal-content').html(data)

                fetch("/Basket/CartProductChangeCount?Id=" + id + "&colorId=" + colorId + "&sizeId=" + sizeId + "&count=" + count)
                    .then(res => res.text())
                    .then(data => {
                        $('.shopping-cart').html(data);

                        fetch("/Basket/RefreshBasketCount")
                            .then(res2 => res2.text())
                            .then(data2 => {
                                $('.header-cart__count').html(data2);
                            })
                    })
            })

   
    })

    //deleteCart
    $(document).on('click', '.shopping-cart__item--remove', function (e) {
        e.preventDefault();
     
        let id = $(this).attr('data-Id');
        let count = $(this).attr('data-count');
        let colorId = $(this).attr('data-colorId');
        let sizeId = $(this).attr('data-sizeId');

        fetch("/Basket/DeletefromCart?Id=" + id + "&colorId=" + colorId + "&sizeId=" + sizeId)
            .then(res => res.text())
            .then(data => {
                $('.shopping-cart').html(data)

                fetch("/Basket/CartProductChangeCount?Id=" + id + "&colorId=" + colorId + "&sizeId=" + sizeId + "&count=" + count)
                    .then(res2 => res2.text())
                    .then(data2 => {
                        $('.basketModal-content').html(data2);

                        fetch("/Basket/RefreshBasketCount")
                            .then(res3 => res3.text())
                            .then(data3 => {
                                $('.header-cart__count').html(data3);
                            })
                    })
            })
    })

    //order
    $(document).on('click', '.accordion-toggle', function () {
        var targetRow = $(this).closest('tr').next('.hiddenRow');
        targetRow.toggleClass('show');
    });

    //addWishlist
    $(document).on('click', '.wishlistBtn', function (e) {
        e.preventDefault();

        let id = $(this).attr('data-Id');
        let productLink = $(this).parent()

        fetch("/Product/AddWishlist?Id=" + id)
            .then(res => res.text())
            .then(data => {
                $('.wishlist__count').html(data)

                fetch("/Product/UpdateProducts?Id=" + id)
                    .then(res => res.text())
                    .then(data => {
                        $(productLink).html(data)
                    })
            })

        $('.addWishlistAlert').addClass('show');

        setTimeout(function () {
            $('.addWishlistAlert').removeClass('show');
        }, 1000);
    })

    //addDetailWishlist
    $(document).on('click', '.wishlistDetailBtn', function (e) {
        e.preventDefault();

        let id = $(this).attr('data-Id');
        let productLink = $(this).parent()

        fetch("/Product/AddWishlist?Id=" + id)
            .then(res => res.text())
            .then(data => {
                $('.wishlist__count').html(data)

                fetch("/Product/UpdateDetailProducts?Id=" + id)
                    .then(res => res.text())
                    .then(data => {
                        $(productLink).html(data)
                    })
            })

        $('.addWishlistAlert').addClass('show');

        setTimeout(function () {
            $('.addWishlistAlert').removeClass('show');
        }, 1000);
    })

    //deleteWishlist
    $(document).on('click', '.product-card__wishlistDeleteBtn', function (e) {
        e.preventDefault();

        let id = $(this).attr('data-Id');

        fetch("/product/DeletefromWishlist?Id=" + id)
            .then(res => res.text())
            .then(data => {
                $('.wishlist-content').html(data)

                fetch("/Product/WishlistCount?Id=" + id)
                    .then(res => res.text())
                    .then(data => {
                        $('.wishlist__count').html(data)
                    })
            })
    })

    //giftCard
    $(document).on('click', '.checkout-gift-card-btn', function (e) {
        e.preventDefault();

        let value = $(".checkout-gift-card").val();
        let shipping = $(this).attr("data-shipping");
        console.log(shipping);
        let subTotal = $(this).attr("data-subTotal");
        console.log(subTotal);

        let content = $(this).parent().parent();

        fetch("/Order/GiftCard?giftCode=" + value + "&shipping1=" + shipping + "&subTotal1=" + subTotal)
            .then(res => res.text())
            .then(data => {
                $(content).html(data)

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
                            if (radioInputs[i].id === "information-ship") {
                                shippingAddress.style.display = 'block';
                                shippingPrice.style.display = 'flex';
                                shippingAddressBreadCrumb.style.display = 'block';
                                pickupLocation.style.display = 'none';
                                pickupPrice.style.display = 'none';
                            } else {
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
            })
    })

})