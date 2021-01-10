using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class Main : MonoBehaviourPunCallbacks {

    int i;
    int time = 0;

    object [] keyarray = new object[128];

    object [] keyboolarray = new object[128];

    [SerializeField]
    
    void Start () {
        MidiBridge.instance.Warmup();
        Debug.Log("MIDI THRU Started");
        //MidiOut.SendProgramChange(MidiChannel.Ch2, 81, 0);
        for(i= 0; i<=127; i++){
            keyboolarray[i] = false;
        }

        Debug.Log("IsMine:" + photonView.IsMine);
    }

    void Update () {
        
        //同時押し
        if (photonView.IsMine) {
            for(i= 0; i<=127; i++){
                keyarray[i] = (float)(MidiInput.GetKey(MidiChannel.Ch1, i));
                //押されてるとき
                if ((float)keyarray[i] != 0){
                    if ((bool)keyboolarray[i] == false){ //押された瞬間
                        Debug.Log("key on:" + i);
                        keyboolarray[i] = true;
                        
                        //MidiOut.SendNoteOn(MidiChannel.Ch2, i, (float)keyarray[i]);

                        photonView.RPC(nameof(SendNoteOn), RpcTarget.All, MidiChannel.Ch2, i, (float)keyarray[i]);
                    }
                } else { //0のとき
                    //離した時
                    if ((bool)keyboolarray[i] == true){
                        Debug.Log("key off:" + i);
                        keyboolarray[i] = false;

                        //MidiOut.SendNoteOff(MidiChannel.Ch2, i);

                        photonView.RPC(nameof(SendNoteOff), RpcTarget.All, MidiChannel.Ch2, i);
                    }

                }
                
            }
        }
        
    }

    [PunRPC]
    public void SendNoteOn(MidiChannel channel, int noteNumber, float velocity)
    {
        int cn = Mathf.Clamp ((int)channel, 0, 15);
        noteNumber = Mathf.Clamp (noteNumber, 0, 127);
        velocity = Mathf.Clamp (127.0f * velocity, 0.0f, 127.0f);
        MidiBridge.instance.Send (0x90 + cn, noteNumber, (int)velocity);
    }

    [PunRPC]
    public void SendNoteOff(MidiChannel channel, int noteNumber)
    {
        int cn = Mathf.Clamp ((int)channel, 0, 15);
        noteNumber = Mathf.Clamp (noteNumber, 0, 127);
        MidiBridge.instance.Send (0x80 + cn, noteNumber, 0);
    }
    
}