using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFLibs.Core.Interfaces
{
	public interface INotifyCollectionChanging
	{
		event NotifyCollectionChangedEventHandler CollectionChanging;
	}
}
