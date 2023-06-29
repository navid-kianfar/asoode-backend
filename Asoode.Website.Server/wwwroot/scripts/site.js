(function($, window, document, undefined){
    const rtl = document.getElementsByTagName('html')[0].getAttribute('dir') === 'rtl';
    
    const win = $(window);
    const allMods = $(".feature");

    allMods.each(function(i, e) {
        const el = $(e);
        if (el.visible(true)) {
            el.addClass("already-visible");
        }
    });

    win.scroll(function(event) {
        allMods.each(function(i, e) {
            const el = $(e);
            if (el.visible(true)) {
                el.addClass("come-in");
            }
        });

    });

    $('.your-work .owl-carousel').owlCarousel({
        rtl:rtl,
        dots:true,
        responsive:{
            0:{
                items:1
            },
            600:{
                items:2
            },
            900:{
                items:3
            },
            1200:{
                items:4
            }
        }
    });
    $('.bx-1 .owl-carousel').owlCarousel({
        rtl:rtl,
        dots:true,
        autoplay:true,
        autoplayTimeout: 4000,
        autoplayHoverPause: true,
        loop: true,
        responsive:{
            0:{
                items:1
            }
        }
    });
    $('.slider .owl-carousel').owlCarousel({
        rtl:rtl,
        dots:true,
        autoplay:true,
        autoplayTimeout: 4000,
        autoplayHoverPause: true,
        loop: true,
        responsive:{
            0:{
                items:1
            }
        }
    });
})(jQuery, window, document);