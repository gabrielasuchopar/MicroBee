﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using MicroBee.Data.Models;

namespace MicroBee.ViewModels
{
    class ItemsViewModel : INotifyPropertyChanged
    {
	    private ObservableCollection<MicroItem> _userItems;

	    public ObservableCollection<MicroItem> UserItems
	    {
		    get => _userItems;
		    set
		    {
			    _userItems = value;
				OnPropertyChanged();
		    }
	    }

	    public ItemsViewModel()
	    {
			UserItems = new ObservableCollection<MicroItem>();
	    }
	    public event PropertyChangedEventHandler PropertyChanged;

	    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }
    }
}
