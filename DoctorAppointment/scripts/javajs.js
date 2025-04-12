// First IntersectionObserver for the cta buttons and other elements
const animationElements = document.querySelectorAll('.cta-btn-appointment, .cta-btn-CheckUp, .cta-btn-Health, .para-Health, .cta-Div-Outer');

const observer1 = new IntersectionObserver((entries) => {
  entries.forEach((entry) => {
    if (entry.intersectionRatio >= 0.4) {
      entry.target.classList.add('animate');
    } else {
      entry.target.classList.remove('animate');
    }
  });
}, {
  threshold: [0.4], // Trigger animation at 40% visibility
});

// Observe all animationElements with the first observer
animationElements.forEach((element) => {
  observer1.observe(element);
});


// Second IntersectionObserver for the .row elements
const elements = document.querySelectorAll('.animation-one');

const observerCallback2 = (entries) => {
  entries.forEach(entry => {
    if (entry.isIntersecting) {
      entry.target.classList.add('animate'); 
      observer2.unobserve(entry.target); // Stop observing once animation is triggered
    }
  });
};

const observer2 = new IntersectionObserver(observerCallback2, {
  threshold: 0.05 // Trigger animation at 5% visibility
});

// Observe all .row elements with the second observer
elements.forEach(element => {
  observer2.observe(element);
});







//third intersection observer


// Select the outer video element and the inner child
const targetElement = document.querySelector('.outer-ani-vido-bck');
const innerChildElement = document.querySelector('.inner-child');

// Callback function for when the element is visible in the viewport
const handleIntersection = (entries, observerThird) => {
  entries.forEach(entry => {
    if (entry.intersectionRatio >= 0.4) {
      // Trigger both animations
      entry.target.style.animation = 'grow 2.5s forwards'; // Start grow animation for outer element
      innerChildElement.style.animation = 'slideIn 3s forwards'; // Start slideIn animation for inner child
      observerThird.unobserve(entry.target); // Stop observing after animations start
    }
  });
};

// Create the observer with a 40% visibility threshold
const observerThird = new IntersectionObserver(handleIntersection, { threshold: 0.4 });

// Start observing the outer video element
observerThird.observe(targetElement);
