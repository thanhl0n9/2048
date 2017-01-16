﻿using System;
using System.Collections;
using UnityEngine;

public class RateReviewMgr : BaseMgr
{
	const string TAG_TIMESRATE = "TAG_PROMOTE_TIMESRATE";
	const string TAG_LASTRATE = "TAG_PROMOTE_LASTRATE";

	int _timesRate;
	int _timesLastRate;

	public RateReviewMgr ()
	{
		_timesRate = PlayerPrefs.GetInt(TAG_TIMESRATE, 0);
		_timesLastRate = PlayerPrefs.GetInt (TAG_TIMESRATE, 0);
	}


	public override void InitWithConfig (Hashtable data) {
		base.InitWithConfig (data);
		if (data == null)
			return;
		data = (Hashtable)data["RateReview"];
		if (data == null)
			return;

		_config.RateReview.Enable = (bool)data["Enable"];
		_config.RateReview.ReviewURL = HDUtils.ParseString (data ["ReviewURL"]);
		_config.RateReview.ContactMail = HDUtils.ParseString (data ["ContactMail"]);
		_config.RateReview.TimesToShow = TypeConvert.ToInt(data["TimesToShow"]);
		_config.RateReview.TimesShowNext = TypeConvert.ToInt(data["TimeShowNext"]);
		_config.RateReview.Language.ParseLanguage((Hashtable)data["Languages"]);

		_didInit = true;
	}

	public override void OnConfigLoadComplete() {
		if (!_didInit)
			return;

		// Log event login

		_didComplete = true;
	}

//	public void IncreateRateTimes() {
//		if (!_config.RateReview.Enable)
//			return;
//		_timesRate++;
//		PlayerPrefs.SetInt (TAG_TIMESRATE, _timesRate);
//		PlayerPrefs.Save ();
//	}

	public bool CanRateReview() {
		if (!_config.RateReview.Enable)
			return false;
		if (_timesRate < _config.RateReview.TimesToShow)
			return false;
		if (_timesLastRate == 0 && _timesRate >= _config.RateReview.TimesToShow)
			return true;
		if (_timesRate - _timesLastRate >= _config.RateReview.TimesShowNext)
			return true;
		return false;
	}

	public void ShowRateReview() {
		if (!_config.RateReview.Enable)
			return;
		InhouseSDK.LanguageV2 language = _config.RateReview.Language.getLanguage ();
		string title = language.GetContent ("Title");
		string message = language.GetContent ("Message");
		string ok1 = language.GetContent ("OK1");
		string ok2 = language.GetContent ("OK2");
		string cancel = language.GetContent ("Cancel");
		InhouseSDK.getInstance ().ShowPopup3 (title, message, ok1, ok2, cancel, _config.RateReview.ReviewURL, (string respone) => {
			_timesLastRate = _timesRate;
			PlayerPrefs.SetInt (TAG_LASTRATE, _timesLastRate);
			PlayerPrefs.Save ();
			if (respone == "1") {
				Application.OpenURL(_config.RateReview.ReviewURL);
			} else if (respone == "2") {
				Application.OpenURL("mailto:" + _config.RateReview.ContactMail +"?subject=Feedback Mr Mariano&body=");
			}
		});
	}

	public void OnGameOver() {
		if (!_config.RateReview.Enable)
			return;
		_timesRate++;
		PlayerPrefs.SetInt (TAG_TIMESRATE, _timesRate);
		PlayerPrefs.Save ();
	}

	public void GoToRate() {
		Application.OpenURL (_config.RateReview.ReviewURL);
	}
}