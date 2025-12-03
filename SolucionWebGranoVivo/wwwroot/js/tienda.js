
document.addEventListener("DOMContentLoaded", () => {
    const productos = document.querySelectorAll(".product-card");
    productos.forEach((card, index) => {
        card.style.opacity = 0;
        card.style.transform = "translateY(20px)";
        setTimeout(() => {
            card.style.transition = "all 0.6s ease";
            card.style.opacity = 1;
            card.style.transform = "translateY(0)";
        }, index * 200); 
    });

   
    productos.forEach(card => {
        card.addEventListener("mouseenter", () => {
            card.style.transform = "scale(1.05)";
            card.style.transition = "transform 0.3s ease";
        });
        card.addEventListener("mouseleave", () => {
            card.style.transform = "scale(1)";
        });
    });

 
    document.querySelectorAll(".product-card button").forEach(btn => {
        btn.addEventListener("click", () => {
            btn.classList.add("clicked");
            setTimeout(() => btn.classList.remove("clicked"), 400);
        });
    });
});
