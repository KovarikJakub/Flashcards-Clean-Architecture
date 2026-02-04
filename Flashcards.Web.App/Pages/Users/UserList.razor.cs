using Flashcards.Common.Enums;
using Flashcards.Common.Models;
using Flashcards.Web.BL.Facades;
using Microsoft.AspNetCore.Components;

namespace Flashcards.Web.App.Pages.Users;

public partial class UserList : PageBase
{
    [Inject] private UserFacade UserFacade { get; set; } = null!;
    private List<UserListModel> Users { get; set; } = Enumerable.Empty<UserListModel>().ToList();
    private bool IsCreating { get; set; }
    private bool IsSubmitting { get; set; }
    private string? ErrorMessage { get; set; }

    private UserDetailModel? _selectedUser;
    private NewUserInputs Input { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await Load();
    }

    private async Task Load()
    {
        Users = await UserFacade.GetAllAsync();
    }

    private void ShowCreateForm()
    {
        IsCreating = true;
        ErrorMessage = null;

        Input = new NewUserInputs();
    }

    private void CancelCreate()
    {
        IsCreating = false;
        ErrorMessage = null;
        Input = null!;
    }

    private async Task CreateUser()
    {
        ErrorMessage = null;
        IsSubmitting = true;

        if (string.IsNullOrWhiteSpace(Input.DisplayName) ||
            string.IsNullOrWhiteSpace(Input.Password) ||
            string.IsNullOrWhiteSpace(Input.IdentityName))
        {
            ErrorMessage = "Please fill in all required fields.";
            return;
        }

        try
        {
            var userCreateModel = new UserCreateModel()
            {
                IdentityName = Input.IdentityName,
                Name = Input.DisplayName,
                Password = Input.Password,
                PhotoUrl = string.IsNullOrWhiteSpace(Input.ProfilePictureUrl) ? null : Input.ProfilePictureUrl,
                Role = RoleType.User
            };

            await UserFacade.Create(userCreateModel);

            IsCreating = false;
            Input = null!;
            await Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error while creating user: {ex.Message}";
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private async Task DeleteUser(UserListModel user)
    {
        try
        {
            await UserFacade.Delete(user.Id);
            // Remove from the Users list immediately to reflect the change in the UI
            Users.Remove(user);
            await Load();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error deleting user: {ex.Message}";
        }
    }

    private record NewUserInputs
    {
        public string DisplayName { get; set; } = string.Empty;
        public string IdentityName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string. Empty;
        public RoleType Role { get; set; } = RoleType.User;
    }
    
    // Modal view methods
    private async Task OpenUserDetails(UserListModel user)
    {
        try
        {
            _selectedUser = await UserFacade.GetByIdAsync(user.Id);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading user details: {ex.Message}";
        }
    }
    
    private void CloseUserDetails()
    {
        _selectedUser = null;
    }
    
    private string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return "?";
        return name.Trim().Substring(0, 1).ToUpper();
    }
}