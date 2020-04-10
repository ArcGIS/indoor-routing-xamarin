﻿using System;
using UIKit;

namespace Esri.ArcGISRuntime.OpenSourceApps.IndoorRouting.iOS.Views
{
    public class PseudoTextFieldButton : UIView
    {
        private readonly UILabel _label;
        private readonly UITapGestureRecognizer _tapRecognizer;

        public string Text
        {
            get => _label?.Text ?? "";
            set
            {
                if (_label == null) { return; }

                if (value != _label.Text)
                {
                    _label.Text = value;
                }
            }
        }

        public PseudoTextFieldButton()
        {
            _label = new UILabel
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = UIColor.LinkColor
            };

            AddSubview(_label);

            NSLayoutConstraint.ActivateConstraints(new[]
            {
                _label.LeadingAnchor.ConstraintEqualTo(LeadingAnchor, 8),
                _label.TopAnchor.ConstraintEqualTo(TopAnchor, 8),
                _label.BottomAnchor.ConstraintEqualTo(BottomAnchor, -8),
                _label.TrailingAnchor.ConstraintEqualTo(TrailingAnchor, -8)
            });

            Layer.CornerRadius = 8;
            Layer.BorderColor = UIColor.SystemGrayColor.CGColor;
            Layer.BorderWidth = 2;
            UserInteractionEnabled = true; //for gesture recognizer
            
            BackgroundColor = UIColor.SystemBackgroundColor;
            ClipsToBounds = true;

            _tapRecognizer = new UITapGestureRecognizer(HandleTap);
            AddGestureRecognizer(_tapRecognizer);
        }

        private void HandleTap()
        {
            Tapped?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Tapped;
    }
}
