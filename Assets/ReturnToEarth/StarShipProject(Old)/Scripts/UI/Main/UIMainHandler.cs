using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 파이어 베이스 기본 학습 지점. 인증 절차 시작 부분으로 작성하려고 함.
// auth 에 대해서 좋은 예시가 된 사이트 https://howtofirebase.com/firebase-authentication-for-web-d58aad62cf6d

public class User
{
    public string username;
    public string email;

    public User()
    {
    }

    public User(string username, string email)
    {
        this.username = username;
        this.email = email;
    }
}

namespace StarShip.UI
{
    public class UIMainHandler : MonoBehaviour
    {
        [SerializeField]
        private InputField inputEmail;

        [SerializeField]
        private InputField inputPW;

        [SerializeField]
        private Button anonymousButton;


        private void Awake()
        {
        }

        public void OnLoginButtonClick()
        {
        }
    }

}
