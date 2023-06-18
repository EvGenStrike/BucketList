using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BucketList
{
    [Activity(Label = "AccountActivity")]
    public class AccountActivity : Activity
    {
        private User user;

        private string changeNameString;

        ImageView backArrow;
        ImageView userPhoto;
        TextView userNameTextView;
        EditText changeNameEditText;
        RelativeLayout registrationDateLayout;
        Button deleteAccountButton;
        
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

        private void SetDeleteAccountButton()
        {
            deleteAccountButton.Click += DeleteAccountButton_Click;
        }

        private void DeleteAccountButton_Click(object sender, EventArgs e)
        {
            var myButton = sender as Button;

            RegisterForContextMenu(myButton);

            OpenContextMenu(myButton);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View view, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, view, menuInfo);

            if (view is Button)
            {
                menu.SetHeaderTitle("Вы уверены, что хотите удалить аккаунт?");

                menu.Add(Menu.None, 0, Menu.None, "Да");
                menu.Add(Menu.None, 1, Menu.None, "Нет");
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            if (item.ItemId == 0)
            {
                DeleteAccount();
                return true;
            }

            return base.OnContextItemSelected(item);
        }

        private void DeleteAccount()
        {
            Extensions.OverwriteUser("");
            Extensions.OverwriteGoals(new List<Goal>());
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
            if (string.IsNullOrEmpty(text))
            {
                userNameTextView.Text = user.UserName;
            }
            else
            {
                userNameTextView.Text = text;
            }
        }

        private void SetBackArrow()
        {
            backArrow.Click += (sender, e) => { OnBackPressed(); };
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

        public override void OnBackPressed()
        {
            Extensions.OverwriteUser(user);
            base.OnBackPressed();
        }

    }
}