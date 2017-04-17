//using UnityEngine;
//using System.Collections;

//public class TapsellAds : MonoBehaviour
//{
//    public const string tapsellKey = "ktqoirkampoamskicdroahjfhltpahokcdbqbochfhaigfomeklfamgaobsshfsfkoebgi";
//    // singleton
//    private static TapsellAds _instance;

//    public static TapsellAds instnace
//    {
//        get { return _instance; }
//        set { _instance = value; }
//    }

//    void Awake()
//    {
//        if (_instance == null)
//        {
//            instnace = this;
//        }
//        //DeveloperInterface.getInstance().init(tapsellKey);
//        DeveloperInterface.instance.init(tapsellKey);

//    }
//    private void Start()
//    {
//        ShowVideo(0);
//    }
//    // Update is called once per frame
//    void Update()
//    {

//    }
//    public void ShowVideo(int playerCurrentScore)
//    {
//        // first we need to check the availability of videos
//        DeveloperInterface.instance.showNewVideo(0, -2, (bool connected, bool isAvailable, int award) =>
//        {
//            Debug.Log(" connected " + connected + " isAvailable " + isAvailable + " award  " + award.ToString());
//        });
//    }
//    public bool CheckForAvailableAds(int minimumAward, bool skippableAds)
//    {
//        int adType = (skippableAds) ? DeveloperInterface.VideoPlay_TYPE_SKIPPABLE : DeveloperInterface.VideoPlay_TYPE_NON_SKIPPABLE;
//        DeveloperInterface.instance.checkCtaAvailability(minimumAward, adType, (bool connected, bool isAvailable) =>
//        {
//            if (!connected)
//            {
//                throw new System.Exception("was not able to connect to internet");
//            }
//            if (!isAvailable)
//            {
//                throw new System.Exception("No Video ad is available right now");
//            }
//        });
//        return true;
//    }
//}
