using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using BucketList.Code.Enums;
using BucketList.Code.Extensions;
using System;

namespace BucketList
{
    [Activity(Label = "AccountActivity")]
    public class AccountActivity : Activity
    {
        private User user;

        private ImageView backArrow;
        private ImageView userPhoto;
        private TextView userNameTextView;
        private EditText changeNameEditText;
        private RelativeLayout registrationDateLayout;
        private Button deleteAccountButton;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_account);
            Initialize();
            SetUserPhoto();
            SetUserNameView();
            SetBackArrow();
            SetChangeName();
            SetRegistrationDate();
            SetDeleteAccountButton();
        }

        public override void OnCreateContextMenu(IContextMenu menu, View view, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, view, menuInfo);

            if (view is Button)
            {
                menu.SetHeaderTitle("Вы уверены, что хотите удалить аккаунт?");

                menu.AddOption(ContextMenuOptions.Yes);
                menu.AddOption(ContextMenuOptions.No);
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            var itemId = (ContextMenuOptions)item.ItemId;
            switch (itemId)
            {
                case ContextMenuOptions.Yes:
                    DeleteAccount();
                    return true;
                default:
                    return base.OnContextItemSelected(item);
            }
        }

        public override void OnBackPressed()
        {
            UpdateUser();
            base.OnBackPressed();
        }

        private void Initialize()
        {
            user = Extensions.GetSavedUser();

            backArrow = FindViewById<ImageView>(Resource.Id.account_screen_back_arrow);
            userPhoto = FindViewById<ImageView>(Resource.Id.account_screen_user_photo);
            userNameTextView = FindViewById<TextView>(Resource.Id.account_screen_user_name_text);
            changeNameEditText = FindViewById<EditText>(Resource.Id.account_screen_change_name_edit_text);
            registrationDateLayout = FindViewById<RelativeLayout>(Resource.Id.account_screen_registration_date_layout);
            deleteAccountButton = FindViewById<Button>(Resource.Id.account_screen_account_delete_button);
        }

        private void UpdateUser()
        {
            user.UserName = userNameTextView.Text;
            Extensions.OverwriteUser(user);
        }

        private void SetDeleteAccountButton()
        {
            deleteAccountButton.Click += DeleteAccountButton_Click;
        }

        private void DeleteAccountButton_Click(object sender, EventArgs e)
        {
            this.CreateContextMenu(deleteAccountButton);
        }

        private void DeleteAccount()
        {
            Extensions.ClearSaves();
            var intent = new Intent(this, typeof(RegistrationActivity));
            StartActivity(intent);
        }

        private void SetUserPhoto()
        {
            userPhoto.SetImage(user.UserPhotoPath);
        }

        private void SetRegistrationDate()
        {
            registrationDateLayout.Click += RegistrationDateLayout_Click;
        }

        private void RegistrationDateLayout_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, user.UserRegistrationDate.ToNiceString(), 0).Show();
        }

        private void SetUserNameView()
        {
            userNameTextView.Text = user.UserName;
        }

        private void SetChangeName()
        {
            changeNameEditText.TextChanged += ChangeNameEditText_TextChanged;
        }

        private void ChangeNameEditText_TextChanged(object sender, EventArgs e)
        {
            var text = changeNameEditText.Text;
            userNameTextView.Text = string.IsNullOrEmpty(text) ? user.UserName : text;
        }

        private void SetBackArrow()
        {
            backArrow.Click += (sender, e) => { OnBackPressed(); };
        }
    }
}