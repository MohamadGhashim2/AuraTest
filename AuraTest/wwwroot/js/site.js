const productContainers = [...document.querySelectorAll
    ('.product-container')];
const nxtBtn = [...document.querySelectorAll('.pre-btn')];
const preBtn = [...document.querySelectorAll('.nxt-btn')];
const productCards = [...document.querySelectorAll('.product-card')];
productContainers.forEach((container, i) => {
    const productCards = [...container.querySelectorAll('.product-card')];
    var currentTranslateX = 0; // Initialize the translation value
    if (screen.width>1400) {
        var transform = 1230;
    } else {
        var transform = 900;

    }
    nxtBtn[i].addEventListener('click', () => {
        console.log(currentTranslateX);
        if (currentTranslateX > 2000) {
            currentTranslateX =3400; // Increment the translation value
            nxtBtn[i].style.display = 'none';
        }
        else {
            nxtBtn[i].style.display = 'flex';
            preBtn[i].style.display = 'flex';
            currentTranslateX += transform; // Increment the translation value
        }
        updateProductCardsTransform(productCards, currentTranslateX);
    });

    preBtn[i].addEventListener('click', () => {
        console.log(currentTranslateX);
        if (currentTranslateX <= 1000) {
            currentTranslateX = 0;
            preBtn[i].style.display = 'none';
        }
        else {
            preBtn[i].style.display = 'flex';
            nxtBtn[i].style.display = 'flex';
            currentTranslateX -= transform; // Decrement the translation value
        }
        updateProductCardsTransform(productCards, currentTranslateX);
    });
});

function updateProductCardsTransform(cards, translateX) {
    cards.forEach(card => {
        card.style.transform = `translateX(${translateX}px)`;
    });
}

