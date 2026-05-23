export function init(container) {
    container.querySelector(".nav-btn-next").addEventListener("click", () => {
        gotoNext(container);
    });

    container.querySelector(".nav-btn-prev").addEventListener("click", () => {
        gotoPrevious(container);
    });
}

function gotoNext(container) {
    const btn = container.querySelector(".nav-btn-prev");
    const btnWidth = btn.getBoundingClientRect().right;
    const contentArea = container.querySelector(".swiper-content");
    const items = contentArea.querySelectorAll(".swiper-item");

    for (let i = 0; i < items.length; i++) {
        const start = Math.floor(items[i].getBoundingClientRect().left);

        if (start - btnWidth > 5) {
            contentArea.scrollLeft += start - btnWidth;
            break;
        }
    }
}

function gotoPrevious(container) {
    const btn = container.querySelector(".nav-btn-prev");
    const btnWidth = btn.getBoundingClientRect().right;
    const contentArea = container.querySelector(".swiper-content");
    const items = contentArea.querySelectorAll(".swiper-item");

    for (let i = items.length - 1; i >= 0; i--) {
        const start = Math.ceil(items[i].getBoundingClientRect().left);

        if (start < (btnWidth - 5)) {
            contentArea.scrollLeft += start - btnWidth;
            break;
        }
    }
}