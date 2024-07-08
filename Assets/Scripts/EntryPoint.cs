using System;
using System.Collections;
using System.Collections.Generic;
using Nakama;
using shGames.Network;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace shGames
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private Button findMatchButton;
        [SerializeField] private Button cancelMatchButton;
        [SerializeField] private Button quitMatchButton;

        [SerializeField] private TextMeshProUGUI playerStatusText;

        [SerializeField] private NakamaConnection nakamaConnection;
        
        private Action<IUserPresence> onMatchJoined;

        private IUserPresence localPlayer;
        private IMatch currentMatch;

        private void Awake()
        {
            findMatchButton.onClick.AddListener(nakamaConnection.FindMatch);
            cancelMatchButton.onClick.AddListener(RequestCancelMatchmaking);
            quitMatchButton.onClick.AddListener(QuitMatch);
            onMatchJoined += OnMatchJoined;
        }

        private void OnMatchJoined(IUserPresence joined)
        {
            if (joined.Equals(localPlayer))
            {
                playerStatusText.text = $"Player: {joined.Username}";
            }
        }

        private async void RequestCancelMatchmaking()
        {
            await nakamaConnection.CancelMatchmaking();
        }

        // Start is called before the first frame update
        private async void Start()
        {
            await nakamaConnection.Connect();

            nakamaConnection.Socket.ReceivedMatchmakerMatched += OnReceivedMatchMakerMatched;
            nakamaConnection.Socket.ReceivedMatchPresence += OnReceivedMatchPresence;
            nakamaConnection.Socket.ReceivedNotification += OnReceivedNotification;
        }

        private async void QuitMatch()
        {
            Debug.Log("Quitting game");
            await nakamaConnection.Socket.LeaveMatchAsync(currentMatch.Id);
            playerStatusText.text = $"Player: {localPlayer.Username} left the match";
            Debug.Log("Quited game");
        }

        #region Socket Event Listeners

        private void OnReceivedNotification(IApiNotification obj)
        {
            Debug.Log("notif: " + obj.Subject);
        }

        private void OnReceivedMatchPresence(IMatchPresenceEvent obj)
        {
            foreach (var joined in obj.Joins)
            {
                Debug.Log("Joined match: " + joined.Username + "\n" + joined.Status + " " + joined.Persistence + " \n" + joined.UserId+" - "+
                          joined.SessionId);
                onMatchJoined.Invoke(joined);
            }

            foreach (var leaved in obj.Leaves)
            {
                Debug.Log("Left match: " + leaved.Username + "\n" + leaved.Status + " " + leaved.Persistence + " \n" + leaved.UserId+" - "+
                          leaved.SessionId);
            }
        }

        private async void OnReceivedMatchMakerMatched(IMatchmakerMatched matchmakerMatched)
        {
            localPlayer = matchmakerMatched.Self.Presence;
            var match = await nakamaConnection.Socket.JoinMatchAsync(matchmakerMatched);
            Debug.Log("Local player: " + localPlayer.Username + " - "+localPlayer.UserId);
            Debug.Log(match + " \n Our session id is: " + match.Self.SessionId);

            foreach (var _user in match.Presences)
            {
                Debug.Log("Connected user: " + _user.UserId);
            }
            var account = await nakamaConnection.Client.GetAccountAsync(nakamaConnection.Session);
            var user = account.User;
            Debug.LogFormat("User id '{0}' username '{1}'", user.Id, user.Username);
            Debug.LogFormat("User wallet: '{0}'", account.Wallet);

            currentMatch = match;
        }

        #endregion
    }
}