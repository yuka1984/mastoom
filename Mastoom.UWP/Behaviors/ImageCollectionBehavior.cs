﻿using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Mastoom.UWP.Behaviors
{
	class ImageCollectionBehavior : Behavior<StackPanel>
	{
		private StackPanel attached;

		#region 依存プロパティ

		/// <summary>
		/// 表示するコンテンツ内容
		/// </summary>
		public string Content
		{
			get
			{
				return (string)this.GetValue(ContentProperty);
			}
			set
			{
				this.SetValue(ContentProperty, value);
			}
		}
		public static readonly DependencyProperty ContentProperty =
			DependencyProperty.RegisterAttached(
				"Content",
				typeof(string),
				typeof(ImageCollectionBehavior),
				new PropertyMetadata(null, (s, e) =>
				{
					var view = s as ImageCollectionBehavior;
					if (view != null)
					{
						view.UpdateContent();
					}
				})
			);

		#endregion

		protected override void OnAttached()
		{
			base.OnAttached();
			this.AttachObject();
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			this.DetachObject();
		}

		private void AttachObject()
		{
			this.DetachObject();

			if (this.AssociatedObject != null)
			{
				this.attached = this.AssociatedObject;
			}
		}

		private void DetachObject()
		{
			if (this.attached != null)
			{
				this.attached = null;
			}
		}

		private void UpdateContent()
		{
			if (this.attached == null)
			{
				return;
			}

			var images = this.attached.Children;
			images.Clear();

			var doc = new XmlDocument();
            try
            {
                doc.LoadXml("<div>" + this.Content.Replace("<br>", "<br/>") + "</div>");
            }
            catch
            {
                return;
            }

            var root = doc.HasChildNodes ? doc.FirstChild : null;
			if (root == null)
			{
				return;
			}

			foreach (XmlNode nodeAtRoot in root.ChildNodes)
			{
				foreach (XmlNode node in nodeAtRoot.ChildNodes)
				{
					if (node is XmlElement element)
					{
						switch (element.Name.ToLower())
						{
							case "a":
								if (element.HasAttribute("href"))
								{
									var link = element.Attributes["href"].Value;
									if (link.Contains("/media/"))
									{
										images.Add(new Image
										{
											MaxHeight = 100,
											Margin = new Thickness(0, 0, 8, 0),
											Source = new BitmapImage(new Uri(link)),
										});
									}
								}
								break;
						}
					}
				}
			}

			// ついでに表示非表示を決める
			this.attached.Visibility = images.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
	}
}
