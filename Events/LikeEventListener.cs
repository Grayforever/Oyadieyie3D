using Android.App;
using Android.Runtime;
using Android.Widget;
using Firebase;
//using Firebase.Firestore;
using Oyadieyie3D.HelperClasses;
using System;

namespace Oyadieyie3D.Events
{
    public class LikeEventListener
    {
        string postID;
        bool Like;

        public LikeEventListener(string _postId)
        {
            postID = _postId;
        }

        //public void LikePost()
        //{
        //    Like = true;
        //    SessionManager.GetFirestore().Collection("posts").Document(postID).Get()
        //        .AddOnCompleteListener(new OncompleteListener((t) =>
        //        {
        //            try
        //            {
        //                if (t.IsSuccessful)
        //                {
        //                    PerformAction(t.Result);
        //                }
        //                else
        //                {
        //                    throw (t.Exception);
        //                }
        //            }
        //            catch (FirebaseFirestoreException ffe)
        //            {

        //                Toast.MakeText(Application.Context, ffe.Message, ToastLength.Short).Show();
        //            }
        //            catch (FirebaseNetworkException fne)
        //            {

        //                Toast.MakeText(Application.Context, fne.Message, ToastLength.Short).Show();
        //            }
        //            catch (Exception e)
        //            {

        //                Toast.MakeText(Application.Context, e.Message, ToastLength.Short).Show();
        //            }
        //        }));

        //}

        //private void PerformAction(Java.Lang.Object result)
        //{
        //    DocumentSnapshot snapshot = (DocumentSnapshot)result;

        //    if (!snapshot.Exists())
        //    {
        //        return;
        //    }

        //    DocumentReference likesReference = SessionManager.GetFirestore().Collection("posts").Document(postID);

        //    if (Like)
        //    {
        //        likesReference.Update("likes." + SessionManager.GetFirebaseAuth().CurrentUser.Uid, true);
        //    }
        //    else
        //    {
        //        if (snapshot.Get("likes") == null)
        //        {
        //            return;
        //        }

        //        var data = snapshot.Get("likes") != null ? snapshot.Get("likes") : null;
        //        if (data != null)
        //        {
        //            var dictionaryFromHashMap = new JavaDictionary<string, string>(data.Handle, JniHandleOwnership.DoNotRegister);

        //            string uid = SessionManager.GetFirebaseAuth().CurrentUser.Uid;

        //            if (dictionaryFromHashMap.Contains(uid))
        //            {
        //                dictionaryFromHashMap.Remove(uid);
        //                likesReference.Update("likes", dictionaryFromHashMap);
        //            }
        //        }

        //    }

        //}

        //public void UnlikePost()
        //{
        //    Like = false;
        //    SessionManager.GetFirestore().Collection("posts").Document(postID).Get()
        //        .AddOnCompleteListener(new OncompleteListener((t) =>
        //        {
        //            try
        //            {
        //                if (t.IsSuccessful)
        //                {
        //                    PerformAction(t.Result);
        //                }
        //                else
        //                {
        //                    throw (t.Exception);
        //                }
        //            }
        //            catch (FirebaseFirestoreException ffe)
        //            {

        //                Toast.MakeText(Application.Context, ffe.Message, ToastLength.Short).Show();
        //            }
        //            catch (FirebaseNetworkException fne)
        //            {

        //                Toast.MakeText(Application.Context, fne.Message, ToastLength.Short).Show();
        //            }
        //            catch (Exception e)
        //            {

        //                Toast.MakeText(Application.Context, e.Message, ToastLength.Short).Show();
        //            }
        //        }));
        //}

    }
}