using Microsoft.AspNetCore.Mvc;

namespace Task2.Controllers;

    public class LoginController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<HomeController> logger;
        private readonly PasswordManager passwordManager;
        private readonly JwtToken jwtTokenGen;

        public LoginController(IUserRepository userRepository, PasswordManager passwordManager, JwtToken jwtTokenGen, ILogger<HomeController> _logger)
        {
            this.userRepository = userRepository;
            this.passwordManager = passwordManager;
            this.jwtTokenGen = jwtTokenGen;
            logger = _logger;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Message = "Username and password are required.";
                return View("Index");
            }

            var existingUser = userRepository.GetUserByName(userName);
            if (existingUser == null)
            {
                ViewBag.Message = "User not found.";
                return View("Index");
            }

            if (!passwordManager.VerifyPasswordHash(password, existingUser.PasswordHash, existingUser.PasswordSalt))
            {
                ViewBag.Message = "Incorrect password.";
                return View("Index");
            }

            var token = jwtTokenGen.GenerateToken(existingUser.Id, existingUser.UserRole.ToString());
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(1)
            };

            Response.Cookies.Append("authToken", token, cookieOptions);
            return RedirectToAction("Index", "Home");
        }
    }

