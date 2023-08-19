//#region pagination-move
function paginationMove() {
    const paginationLinks = document.querySelectorAll('.pagination__item');
    const prevBtn = document.querySelector('.pagination__prev-btn');
    const nextBtn = document.querySelector('.pagination__next-btn');

    paginationLinks.forEach(link => {
        link.addEventListener('click', (event) => {
            event.preventDefault();

            // Aktif sınıfı diğer tüm linklerden kaldır
            paginationLinks.forEach(link => {
                link.classList.remove('pagination-active');
            });

            // Seçilen linki aktif yap
            link.classList.add('pagination-active');
        });
    });

    prevBtn.addEventListener('click', (event) => {
        event.preventDefault();

        const activeLink = document.querySelector('.pagination__item.pagination-active');
        const prevLink = activeLink.previousElementSibling;

        if (prevLink && !prevLink.classList.contains('pagination__prev-btn')) {
            activeLink.classList.remove('pagination-active');
            prevLink.classList.add('pagination-active');
        }
    });

    nextBtn.addEventListener('click', (event) => {
        event.preventDefault();

        const activeLink = document.querySelector('.pagination__item.pagination-active');
        const nextLink = activeLink.nextElementSibling;

        if (nextLink && !nextLink.classList.contains('pagination__next-btn')) {
            activeLink.classList.remove('pagination-active');
            nextLink.classList.add('pagination-active');
        }
    });


}
paginationMove();
//#endregion
