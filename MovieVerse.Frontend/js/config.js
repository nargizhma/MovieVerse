window.MV = window.MV || {};
MV.config = Object.freeze({
  API_BASE_URL: "http://localhost:5033/api",
  BACKEND_ORIGIN: "http://localhost:5033",
  EMAIL_VERIFICATION_HUB_URL:
  "http://localhost:5033/hubs/email-verification",
  TOKEN_KEY: "movieverse_jwt",
  FLASH_KEY: "movieverse_flash",

  // Generic fallback: user profiles and any image that is not specifically
  // a title poster/episode image or an actor/director/writer portrait.
  PLACEHOLDER_IMAGE: "assets/images/placeholder.png",

  // Keep the extension here equal to the real file extension in assets/images.
  PEOPLE_PLACEHOLDER_IMAGE: "assets/images/people-placeholder.png",
  MOVIE_PLACEHOLDER_IMAGE: "assets/images/movie-placeholder.jpg"
});
