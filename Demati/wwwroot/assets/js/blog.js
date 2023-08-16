//#region blog-tablist
function blogTabList() {
  let blogTabLi = document.querySelectorAll('.blog-tab-list__li');
  let blogTabLiContent = document.querySelectorAll('.blog-tab-list__content')

  blogTabLi.forEach((item, index) => {
    item.addEventListener('click', () => {
      blogTabLi.forEach(item => {
        item.classList.remove('active-b-selected')
      })
      item.classList.add('active-b-selected');

      blogTabLiContent.forEach(item => {
        item.classList.remove("active-b-content");
      })
      blogTabLiContent[index].classList.add('active-b-content');

    })
  });
}
blogTabList();
//#endregion