using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARScaleController : MonoBehaviour {

  ARSessionOrigin m_ARSessionOrigin;
  public Slider scaleSlider;

  private void Awake () {
    m_ARSessionOrigin = GetComponent<ARSessionOrigin> ();
  }

  // Start is called before the first frame update
  void Start () {
    scaleSlider.onValueChanged.AddListener (OnSliderValueChanged);
  }

  // Update is called once per frame
  void Update () { }

  public void OnSliderValueChanged (float value) {
    if (scaleSlider != null) {
      m_ARSessionOrigin.transform.localScale = Vector3.one / value;
    }
  }
}
