using UnityEngine;

namespace ZABsters {

    public class KeyPressTask : ITask
    {
        //the options for keys are w, a, s, d. make it so that
        //theres a public variable thats a dropdown menu with those options:

        public KeyCode key;

        public override bool CheckTool()
        {
            //bascally check input.getkeydown(keyoption)
            //and then return true if it is pressed:
            if (Input.GetKeyDown(key))
            {
                //todo: change dis around, if we want extra task.
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
