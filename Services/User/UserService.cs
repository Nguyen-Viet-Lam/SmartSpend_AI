using Microsoft.EntityFrameworkCore;
using SmartSpendAI.Models;
using SmartSpendAI.Models.Dtos.User;
using SmartSpendAI.Security;

namespace SmartSpendAI.Services.Users
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<UserProfileServiceResult> GetProfileAsync(int userId, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (user is null)
            {
                return new UserProfileServiceResult
                {
                    Success = false,
                    Message = "Không tìm thấy người dùng."
                };
            }

            return new UserProfileServiceResult
            {
                Success = true,
                Response = new ProfileResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    AvatarUrl = user.AvatarUrl,
                    IsLocked = user.IsLocked,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<UserProfileServiceResult> UpdateProfileAsync(
            int userId,
            UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (user is null)
            {
                return new UserProfileServiceResult
                {
                    Success = false,
                    Message = "Không tìm thấy người dùng."
                };
            }

            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var fullName = request.FullName.Trim();

            var emailInUse = await _dbContext.Users
                .AnyAsync(x => x.UserId != userId && x.Email == normalizedEmail, cancellationToken);

            if (emailInUse)
            {
                return new UserProfileServiceResult
                {
                    Success = false,
                    Message = "Email đã được sử dụng bởi tài khoản khác."
                };
            }

            user.FullName = fullName;
            user.Email = normalizedEmail;
            user.AvatarUrl = string.IsNullOrWhiteSpace(request.AvatarUrl) ? null : request.AvatarUrl.Trim();

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UserProfileServiceResult
            {
                Success = true,
                Message = "Cập nhật hồ sơ thành công.",
                Response = new ProfileResponse
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    AvatarUrl = user.AvatarUrl,
                    IsLocked = user.IsLocked,
                    IsEmailVerified = user.IsEmailVerified,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<UserProfileServiceResult> ChangePasswordAsync(
            int userId,
            ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

            if (user is null)
            {
                return new UserProfileServiceResult
                {
                    Success = false,
                    Message = "Không tìm thấy người dùng."
                };
            }

            if (!PasswordHashUtility.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            {
                return new UserProfileServiceResult
                {
                    Success = false,
                    Message = "Mật khẩu hiện tại không đúng."
                };
            }

            if (!HasStrongPassword(request.NewPassword))
            {
                return new UserProfileServiceResult
                {
                    Success = false,
                    Message = "Mật khẩu mới phải có ít nhất 8 ký tự, gồm chữ hoa, chữ thường và chữ số."
                };
            }

            user.PasswordHash = PasswordHashUtility.HashPassword(request.NewPassword);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UserProfileServiceResult
            {
                Success = true,
                Message = "Đổi mật khẩu thành công."
            };
        }

        private static bool HasStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                return false;
            }

            return password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit);
        }
    }
}
