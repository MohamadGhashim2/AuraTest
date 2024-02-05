/*product*/
const productContainers = [...document.querySelectorAll
    ('.product-container')];
const nxtBtn = [...document.querySelectorAll('.nxt-btn')];
const preBtn = [...document.querySelectorAll('.pre-btn')];

productContainers.forEach((item, i) => {
    let containerDeimensions = item.getBoundingClientRect();
    let containerWidth = containerDeimensions.width;
    nxtBtn[i].addEventListener('click', () => {
        item.scrollLeft += containerWidth/1.3;
    })
    preBtn[i].addEventListener('click', () => {
        item.scrollLeft -= containerWidth/1.3;
    })
});