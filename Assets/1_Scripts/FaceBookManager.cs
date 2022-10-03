using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 페이스북 로그인의 순서 : 초기화 -> 로그인

public class FaceBookManager : MonoBehaviour
{
    [SerializeField] RawImage _pic;
    [SerializeField] Text _nameText;
    [SerializeField] Text _log;
    GameObject _button;
    string _name;

    #region [초기화]
    // 페이스북 초기화
    public void InitFacebook()
    {
        if (!FB.IsInitialized)
        {
            Debug.Log("페이스북 SDK 초기화 시작");
            FB.Init(InitCallBack, OnHideUnity);
        }
        else
        {
            Debug.Log("페이스북 SDK 초기화 완료");
            _button.transform.GetChild(0).GetComponent<Text>().text = "페이스북 로그인";
            _log.text = "초기화 완료";
            FB.ActivateApp();
        }
    }

    // 초기화 명령을 실행한 뒤, 호출되는 함수
    void InitCallBack()
    {
        if (FB.IsInitialized)
        {
            Debug.Log("페이스북 SDK 초기화 완료");
            _button.transform.GetChild(0).GetComponent<Text>().text = "페이스북 로그인";
            _log.text = "초기화 완료";
            FB.ActivateApp();
        }
        else
            Debug.Log("페이스북 SDK 초기화 실패");
    }

    // 초기화 명령 실행 도중 호출되는 함수
    // 페이스북 로그인 중 유니티를 잠시 멈추게 하는 기능을 구현
    void OnHideUnity(bool isGameShow)
    {
        if (isGameShow)
            Time.timeScale = 1;
        else
            Time.timeScale = 0;
    }
    #endregion

    #region [로그인]
    void LogInFaceBook()
    {
        if (!FB.IsLoggedIn)
        {
            // 권한
            List<string> parms = new List<string>() { "public_profile", "email" };
            FB.LogInWithReadPermissions(parms, AuthCallBack);
        }
    }

    void AuthCallBack(ILoginResult result)
    {
        if(result.Error != null)
            Debug.LogFormat("Auth Error : {0}", result.Error);
        else  // 메뉴
            DealWithFBMenus(FB.IsLoggedIn);
        
    }

    void DealWithFBMenus(bool isLoggedIn)
    {
        if (isLoggedIn)
        {
            // 쿼리를 이용한 정보 가져오기
            FB.API("/me?fields=first_name", HttpMethod.GET, SettingUserName);
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, SettingProfilePic);
            _button.transform.GetChild(0).GetComponent<Text>().text = "페이스북 로그아웃";
            _log.text = "로그인 완료";
        }
    }

    void SettingUserName(IResult ret)
    {
        if(ret.Error == null)
        {
            _name = "Name : " + ret.ResultDictionary["first_name"];
            _name += "\nID : " + ret.ResultDictionary["id"];
        }
        else
            Debug.LogFormat("UserName Error : {0}", ret.Error);
    }
    void SettingProfilePic(IGraphResult ret)
    {
        if (ret.Error == null)
        {
            _pic.texture = ret.Texture;
            _nameText.text = _name;
        }
        else
            Debug.LogFormat("UserProfile Picture Error : {0}", ret.Error);
    }
    #endregion

    #region [로그아웃]
    void LogOutFaceBook()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();
            _pic.texture = null;
            _nameText.text = "로그인하세요";
        }

    }
    #endregion


    void OnGUI()
    {
        //if (!FB.IsInitialized && !FB.IsLoggedIn)
        //{
        //    if (GUI.Button(new Rect(0, 0, 180, 45), "FaceBook 초기화"))
        //        InitFacebook();
        //}
        //else if(!FB.IsLoggedIn)
        //    if (GUI.Button(new Rect(Screen.width - 180, 0, 180, 45), "FaceBook 로그인"))
        //    LogInFaceBook();



        //if (FB.IsLoggedIn)
        //    GUI.Box(new Rect(100, 300, 180, 30), _name);
        //else
        //    if (GUI.Button(new Rect(Screen.width - 180, 0, 180, 45), "FaceBook 로그인"))
        //        LogInFaceBook();
    }

    public void ButtonClicked()
    {
        _button = EventSystem.current.currentSelectedGameObject;

        //Debug.Log(_initButton.name);
        if (!FB.IsInitialized && !FB.IsLoggedIn)
            InitFacebook();
        else if (FB.IsInitialized && !FB.IsLoggedIn)
            LogInFaceBook();
        else
            LogOutFaceBook();
    }
}
