using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages.Users;

public partial class UserList : PageBase
{
    [Inject]
    private UserFacade UserFacadeService { get; set; } = null!;

    private List<UserListModel>? Users { get; set; }
    private bool IsCreating { get; set; }
    private bool IsSubmitting { get; set; }
    private string? ErrorMessage { get; set; }
    private NewUserModel NewUser { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await Load();
        await base.OnInitializedAsync();
    }

    private async Task Load()
    {
        Users = (await UserFacadeService.GetAllAsync()).ToList();
    }

    private void ShowCreateForm()
    {
        IsCreating = true;
        ErrorMessage = null;
        NewUser = new NewUserModel 
        { 
            Role = Flashcards.Common.Enums.RoleType.User 
        };
    }

    private void CancelCreate()
    {
        IsCreating = false;
        ErrorMessage = null;
        NewUser = new NewUserModel();
    }

    private async Task CreateUser()
    {
        ErrorMessage = null;
        IsSubmitting = true;

        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(NewUser.UserName))
            {
                ErrorMessage = "Username is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUser.Password))
            {
                ErrorMessage = "Password is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUser.Email))
            {
                ErrorMessage = "Email is required";
                return;
            }

            if (string.IsNullOrWhiteSpace(NewUser.FullName))
            {
                ErrorMessage = "Full Name is required";
                return;
            }

            // Use UserFacade which includes authentication
            var userId = await UserFacadeService.CreateUserWithIdentityAsync(
                userName: NewUser.UserName,
                password: NewUser.Password,
                email: NewUser.Email,
                name: NewUser.FullName,
                photographyUrl: string.IsNullOrWhiteSpace(NewUser.ProfilePicUrl) ? null : NewUser.ProfilePicUrl,
                role: NewUser.Role);

            // Reset form and reload users
            IsCreating = false;
            NewUser = new NewUserModel();
            await Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error creating user: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private void AddNewUser()
    {
        var user = new UserListModel 
        { 
            Id = Guid.NewGuid(), 
            Name = "", 
            Role = Flashcards.Common.Enums.RoleType.User 
        };
        Users ??= new List<UserListModel>();
        Users.Insert(0, user);
    }

    private async Task DeleteUser(UserListModel user)
    {
        if (user.Id != Guid.Empty)
        {
            try
            {
                // Use UserFacade which includes authentication
                await UserFacadeService.DeleteUserWithIdentityAsync(user.Id);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error deleting user: {ex.Message}";
                return;
            }
        }
        Users?.Remove(user);
    }

    private async Task SaveAll()
    {
        if (Users == null) return;

        ErrorMessage = null;

        try
        {
            foreach (var u in Users)
            {
                var detail = new UserDetailModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    PhotographyUrl = null,
                    Role = u.Role
                };

                await UserFacadeService.UpsertAsync(detail);
            }

            await Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error saving users: {ex.Message}";
        }
    }

    private class NewUserModel
    {
        public string? FullName { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? ProfilePicUrl { get; set; }
        public Flashcards.Common.Enums.RoleType Role { get; set; }
    }
}
