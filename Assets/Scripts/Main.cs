using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class Main : MonoBehaviourPunCallbacks, IPunObservable {

    int i;
    int time = 0;

    object [] keyarray = new object[128];

    object [] keyboolarray = new object[128];

    void Start () {
        MidiBridge.instance.Warmup();
        Debug.Log("MIDI THRU Started");
        //MidiOut.SendProgramChange(MidiChannel.Ch2, 81, 0);
        for(i= 0; i<=127; i++){
            keyboolarray[i] = false;
        }
    }

    void Update () {
        //同時押し
        //if (photonView.IsMine) {
            for(i= 0; i<=127; i++){
                keyarray[i] = (float)(MidiInput.GetKey(MidiChannel.Ch1, i));
                //押されてるとき
                if ((float)keyarray[i] != 0){
                    if ((bool)keyboolarray[i] == false){ //押された瞬間
                        Debug.Log("key on:" + i);
                        keyboolarray[i] = true;
                        //MidiOut.SendNoteOn(MidiChannel.Ch2, i, 110);
                        MidiOut.SendNoteOn(MidiChannel.Ch2, i, (float)keyarray[i]);

                    }
                } else { //0のとき
                    //離した時
                    if ((bool)keyboolarray[i] == true){
                        Debug.Log("key off:" + i);
                        keyboolarray[i] = false;
                        MidiOut.SendNoteOff(MidiChannel.Ch2, i);
                    }

                }
                
            }
        //}
        
    }


    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            // 自身側が生成したオブジェクトの場合は
            // 色相値と移動中フラグのデータを送信する
            stream.SendNext(keyarray);
            stream.SendNext(keyboolarray);
            Debug.Log("Sended!");
        } else {
            // 他プレイヤー側が生成したオブジェクトの場合は
            // 受信したデータから色相値と移動中フラグを更新する
            keyarray = (object[])stream.ReceiveNext();
            keyboolarray = (object[])stream.ReceiveNext();
            Debug.Log("Recieved!");

        }
    }
}