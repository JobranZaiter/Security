using Microsoft.AspNetCore.Mvc;

namespace YourNamespace.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserRepository userRepository;
        private readonly PasswordManager passwordManager;

        public RegisterController(IUserRepository userRepository, PasswordManager passwordManager)
        {
            this.userRepository = userRepository;
            this.passwordManager = passwordManager;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
            {
                ViewBag.Message = "Please enter all required credentials.";
                return View("Index");
            }

            var existingUser = userRepository.GetUserByName(request.UserName);
            if (existingUser != null)
            {
                ViewBag.Message = "Choose a different username, it already exists.";
                return View("Index");
            }

            passwordManager.CreatePasswordHasher(request.Password, out string PasswordHash, out string PasswordSalt);

            var user = new User
            {
                UserName = request.UserName,
                PasswordHash = PasswordHash,
                PasswordSalt = PasswordSalt,
                Age = request.Age,
            };

            userRepository.AddUser(user);
            ViewBag.Message = "Account successfully registered. You can now log in!";
            return RedirectToAction("Index", "Login");
        }
    }
}
