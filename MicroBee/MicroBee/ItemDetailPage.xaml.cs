﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MicroBee
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ItemDetailPage : ContentPage
	{
		private int _itemId;
		/// <summary>
		/// Id of the item detail
		/// </summary>
		public int ItemId
		{
			get => _itemId;
			set
			{
				_itemId = value;
				Initialize();
			}
		}

		private bool _isEditEnabled;
		private bool _isAcceptEnabled;
		/// <summary>
		/// Determines whether it is allowed to edit
		/// </summary>
		public bool IsEditEnabled
		{
			get => _isEditEnabled;
			set
			{
				_isEditEnabled = value;
				EditButton.IsVisible = IsEditEnabled;
			}
		}
		/// <summary>
		/// Determines whether it is allowed to accept the job
		/// </summary>
		public bool IsAcceptEnabled
		{
			get => _isAcceptEnabled;
			set
			{
				_isAcceptEnabled = value;
				AcceptJobButton.IsVisible = IsAcceptEnabled;
			}
		}

		/// <summary>
		/// Returns true if IsEditEnabled and the page is in edit mode
		/// </summary>
		private bool _isEditing;
		public bool IsEditing
		{
			get => IsEditEnabled && _isEditing;
			set
			{
				_isEditing = value;
				editLayout.IsVisible = IsEditing;
				detailLayout.IsVisible = !IsEditing;

				OnPropertyChanged();
			}
		}

		public ItemDetailPage()
		{
			InitializeComponent();
			IsEditEnabled = false;
			IsAcceptEnabled = false;
		}

		private async void Initialize()
		{
			await UpdateModelAsync();
		}

		protected override async void OnAppearing()
		{
			await UpdateModelAsync();
		}

		private async void AcceptJobButton_OnClicked(object sender, EventArgs e)
		{
			//accepts job
			if (!App.IsUserAuthenticated)
			{
				throw new InvalidOperationException("User must be logged in to accept the job");
			}
			else
			{
				await App.ItemService.SetAsWorkerAsync(Model.Item.Id);
				await UpdateModelAsync();
			}
		}

		private async Task UpdateModelAsync()
		{
			//gets the updated data

			var item = await App.ItemService.GetMicroItemAsync(ItemId);
			Model.Item = item;

			if (item.ImageId != null)
			{
				byte[] imageData = await App.ItemService.GetImageAsync(item.ImageId.Value);
				Model.ImageData = imageData;
			}

			Model.Categories = await App.ItemService.GetCategoriesAsync();

			Model.UpdateUser();
		}

		private void EditButton_OnClicked(object sender, EventArgs e)
		{
			//changes page from detail to edit
			IsEditing = true;
		}

		private async void SubmitEditButton_OnClicked(object sender, EventArgs e)
		{
			//upload with or without image
			if (Model.IsImageChanged)
			{
				await App.ItemService.UpdateMicroItemAsync(Model.Item, Model.ImageData);
			}
			else
			{
				await App.ItemService.UpdateMicroItemAsync(Model.Item, null);
			}

			//updated data
			await UpdateModelAsync();

			Model.IsImageChanged = false;
			IsEditing = false;
		}

		private async void ChooseImage_OnClicked(object sender, EventArgs e)
		{

			var file = await CrossMedia.Current.PickPhotoAsync();
			if (file == null)
			{
				return;
			}

			using (MemoryStream stream = new MemoryStream())
			{
				file.GetStream().CopyTo(stream);
				Model.ImageData = stream.ToArray();
			}

			//image should be uploaded as well
			Model.IsImageChanged = true;
		}
	}
}