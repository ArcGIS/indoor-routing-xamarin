﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.OpenSourceApps.IndoorRouting.iOS.Helpers;
using Esri.ArcGISRuntime.OpenSourceApps.IndoorRouting.ViewModels;
using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using UIKit;

namespace Esri.ArcGISRuntime.OpenSourceApps.IndoorRouting.iOS.Views
{
    public class RouteResultCard : UIView
    {
        private SelfSizedTableView _stopsTable;
        private UIImageView _travelModeImageView;
        private UILabel _routeDurationLabel;
        private UIButton _closeButton;
        private UILabel _headerLabel;

        public RouteResultCard()
        {
            _stopsTable = new SelfSizedTableView
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                //TableFooterView = null,
                ScrollEnabled = false,
                BackgroundColor = UIColor.Clear,
                AllowsSelection = false
            };

            // Future - consider supporting more travel modes?
            _travelModeImageView = new UIImageView(UIImage.FromBundle("walking")) { TranslatesAutoresizingMaskIntoConstraints = false };
            _travelModeImageView.TintColor = UIColor.LabelColor;
            _travelModeImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            _routeDurationLabel = new UILabel { TranslatesAutoresizingMaskIntoConstraints = false };

            _closeButton = new CloseButton { TranslatesAutoresizingMaskIntoConstraints = false };

            _headerLabel = new UILabel { TranslatesAutoresizingMaskIntoConstraints = false, Text = "RouteResultHeader".AsLocalized() };
            _headerLabel.Font = UIFont.BoldSystemFontOfSize(28);
            _headerLabel.TextColor = UIColor.LabelColor;

            this.AddSubviews(_stopsTable, _travelModeImageView, _routeDurationLabel, _closeButton, _headerLabel);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                // result header
                _headerLabel.TopAnchor.ConstraintEqualTo(this.TopAnchor, 8),
                _headerLabel.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor, 8),
                _headerLabel.TrailingAnchor.ConstraintEqualTo(_closeButton.LeadingAnchor, -8),
                //clear button
                _closeButton.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor, -8),
                _closeButton.CenterYAnchor.ConstraintEqualTo(_headerLabel.CenterYAnchor),
                _closeButton.WidthAnchor.ConstraintEqualTo(32),
                _closeButton.HeightAnchor.ConstraintEqualTo(32),
                // stops view
                _stopsTable.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor),
                _stopsTable.TopAnchor.ConstraintEqualTo(_routeDurationLabel.BottomAnchor, 8),
                _stopsTable.TrailingAnchor.ConstraintEqualTo(this.TrailingAnchor, -8),
                // image
                _travelModeImageView.LeadingAnchor.ConstraintEqualTo(this.LeadingAnchor, 8),
                _travelModeImageView.TopAnchor.ConstraintEqualTo(_routeDurationLabel.TopAnchor),
                _travelModeImageView.BottomAnchor.ConstraintEqualTo(_routeDurationLabel.BottomAnchor),
                _travelModeImageView.WidthAnchor.ConstraintEqualTo(32),
                // walk time label
                _routeDurationLabel.TopAnchor.ConstraintEqualTo(_headerLabel.BottomAnchor, 8),
                _routeDurationLabel.LeadingAnchor.ConstraintEqualTo(_travelModeImageView.TrailingAnchor, 8),
                //
                this.BottomAnchor.ConstraintEqualTo(_stopsTable.BottomAnchor, 8)
            });


            _closeButton.TouchUpInside += Close_Clicked;

            AppStateViewModel.Instance.PropertyChanged += AppState_PropertyChanged;
        }

        private void AppState_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(AppStateViewModel.CurrentRoute))
            {
                return;
            }

            RouteResult routeResult = AppStateViewModel.Instance.CurrentRoute;

            if (routeResult == null)
            {
                _stopsTable.Source = null;
                return;
            }

            StringBuilder walkTimeStringBuilder = new StringBuilder();

            Route firstReoute = routeResult.Routes.First();

            // Add walk time and distance label
            // TODO - improve this
            if (firstReoute.TotalTime.Hours > 0)
            {
                walkTimeStringBuilder.Append(string.Format("{0} h {1} m", firstReoute.TotalTime.Hours, firstReoute.TotalTime.Minutes));
            }
            else
            {
                walkTimeStringBuilder.Append(string.Format("{0} min", firstReoute.TotalTime.Minutes + 1));
            }

            var tableSource = new List<Feature>() { AppStateViewModel.Instance.FromLocationFeature, AppStateViewModel.Instance.ToLocationFeature };

            _stopsTable.Source = new RouteTableSource(tableSource);
            _stopsTable.ReloadData(); // TODO - is this necessary?

            _routeDurationLabel.Text = walkTimeStringBuilder.ToString();
        }

        private void Close_Clicked(object sender, EventArgs e) => AppStateViewModel.Instance.TransitionToState(AppStateViewModel.UIState.PlanningRoute);
    }
}