﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroBee.Data.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MicroBee
{
	/// <summary>
	/// Represents a page of user created jobs
	/// </summary>
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserItemsPage : ContentPage
	{
		public UserItemsPage()
		{
			InitializeComponent();
		}

		protected override async void OnAppearing()
		{
			//profile is initialized/refreshed on appearing
			
			var profile = await App.AccountService.GetUserProfileAsync();
			Model.UserItems = new ObservableCollection<MicroItem>(profile.CreatedItems);
		}

		private void ListView_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
		{
			//carousel page of user created jobs

			var selectedItem = (MicroItem)((ListView)sender).SelectedItem;
			if (selectedItem == null)
			{
				return; ;
			}

			ItemCarouselPage page = new ItemCarouselPage();
			foreach (int itemId in Model.UserItems.Select(i => i.Id))
			{
				page.Children.Add(new ItemDetailPage()
				{
					ItemId = itemId,
					IsEditEnabled = true
				});
			}

			page.CurrentPage = page.Children.FirstOrDefault(p => ((ItemDetailPage)p).ItemId == selectedItem.Id);
			Navigation.PushAsync(page);
		}

		private void AddButton_OnClicked(object sender, EventArgs e)
		{
			// add a new item (page)

			var addPage = new AddItemPage();
			addPage.AddSucceeded += async (sendA, eA) =>
			{
				await Navigation.PopAsync();
			};

			Navigation.PushAsync(addPage);
		}
	}
}