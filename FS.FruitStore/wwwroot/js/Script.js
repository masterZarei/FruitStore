const toTop = document.querySelector('.to-top');
window.addEventListener('scroll', () => {
    if (window.pageYOffset > 400) {
        toTop.classList.add('active');
    } else {
        toTop.classList.remove('active');
    }
});
const $dropdown = $(".dropdown");
const $dropdownToggle = $(".dropdown-toggle");
const $dropdownMenu = $(".dropdown-menu");
const showClass = "show";
$(window).on("load resize", function () {
    if (this.matchMedia("(min-width: 768px)").matches) {
        $dropdown.hover(
            function () {
                const $this = $(this);
                $this.addClass(showClass);
                $this.find($dropdownToggle).attr("aria-expanded", "true");
                $this.find($dropdownMenu).addClass(showClass);
            },
            function () {
                const $this = $(this);
                $this.removeClass(showClass);
                $this.find($dropdownToggle).attr("aria-expanded", "false");
                $this.find($dropdownMenu).removeClass(showClass);
            }
        );
    } else {
        $dropdown.off("mouseenter mouseleave");
    }
});
// slider products
let thumbnail = document.getElementsByClassName('thumbnail');
let slider = document.getElementById('slider');
let leftButton = document.getElementById('left-slide');
let rightButton = document.getElementById('right-slide');
leftButton.addEventListener('click', function () {
    slider.scrollLeft -= 125;
})
rightButton.addEventListener('click', function () {
    slider.scrollLeft += 125;
})
const maxScrollLeft = slider.scrollWidth - slider.clientWidth;
function autoPlay() {
    if (slider.scrollLeft > (maxScrollLeft - 1)) {
        slider.scrollLeft -= maxScrollLeft;
    } else {
        slider.scrollLeft += 1
    }
}
let play = setInterval(autoPlay, 50)
for (let i = 0; i < thumbnail.length; i++) {
    thumbnail[i].addEventListener('mouseover', () => {
        clearInterval(play)
    })
    thumbnail[i].addEventListener('mouseout', () => {
        return play = setInterval(autoPlay, 50);
    })
}














const detProImg = document.querySelectorAll('.detPro-img');
const active_img = document.querySelector('.active-image');

for (let i = 0; i < detProImg.length; i++) {
    detProImg[i].onclick = (e) => {
        for(let a = 0; a < detProImg.length; a++) {
            detProImg[a].classList.remove('active')
        }
        detProImg[i].classList.add('active');
      
        active_img.src = e.target.src;
        
    }
    
}




















