# Periodic Mixture Modelling
Bayesian Modelling of the Temporal Aspects of Smart Home Activity with Circular Statistics. 

Code accompanying the paper:

    @inproceedings{diethe2015bayesian,
        title={Bayesian modelling of the temporal aspects of smart home activity with circular statistics},
        author={Diethe, Tom and Twomey, Niall and Flach, Peter},
        booktitle={Joint European Conference on Machine Learning and Knowledge Discovery in Databases},
        pages={279--294},
        year={2015},
        organization={Springer}
    }


## Bayesian Modelling of the Temporal Aspects of Smart Home Activity with Circular Statistics


Tom Diethe, Niall Twomey and Peter Flach

Intelligent Systems Laboratory, University of Bristol, UK

### Abstract
Typically, when analysing patterns of activity in a smart-home environment, the daily patterns of activity are either ignored completely or summarised into a high-level "hour-of-day" feature that is then combined with sensor activities. However, when summarising the temporal nature of an activity into a coarse feature such as this, not only is information lost after discretisation, but also the strength of the periodicity of the action is ignored. We propose to model the temporal nature of activities using circular statistics, and in particular by performing Bayesian inference with Wrapped Normal (WN) and WN Mixture (WNM) models. We firstly demonstrate the accuracy of inference on toy data using both Gibbs sampling and Expectation Propagation (EP), and then show the results of the inference on publicly available smart-home data. Such models can be useful for analysis or prediction in their own right, or can be readily combined with larger models incorporating multiple modalities of sensor activity.


## Infer.NET
This code uses [Infer.NET](http://infernet.azurewebsites.net/default.aspx) to perform inference. Note that whilst this code is released under the MIT license, Infer.NET has [it's own license](http://infernet.azurewebsites.net/docs/Frequently%20Asked%20Questions.aspx).
