export function init(container) {
    container.querySelector(".nav-btn-next").addEventListener("click", () => {
        gotoNext(container);
    });

    container.querySelector(".nav-btn-prev").addEventListener("click", () => {
        gotoPrevious(container);
    });
}

function getContext(container) {
    return {
        btnWidth: container.querySelector(".nav-btn-prev").getBoundingClientRect().right,
        contentArea: container.querySelector(".swiper-content"),
        items: container.querySelectorAll(".swiper-content .swiper-item"),
    };
}

function gotoNext(container) {
    const { btnWidth, contentArea, items } = getContext(container);

    for (let i = 0; i < items.length; i++) {
        const start = Math.floor(items[i].getBoundingClientRect().left);

        if (start - btnWidth > 5) {
            contentArea.scrollLeft += start - btnWidth;
            break;
        }
    }
}

function gotoPrevious(container) {
    const { btnWidth, contentArea, items } = getContext(container);

    for (let i = items.length - 1; i >= 0; i--) {
        const start = Math.ceil(items[i].getBoundingClientRect().left);

        if (start < (btnWidth - 5)) {
            contentArea.scrollLeft += start - btnWidth;
            break;
        }
    }
}