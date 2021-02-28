using System.Collections.Generic;

[System.Serializable]
public class SceneDescriptor
{
    public string id;
    public MusicDescriptor music;
    public MusicDescriptor ambiance1;
    public MusicDescriptor ambiance2;
    public AnimatedImageDescriptor background;
    public AnimatedImageDescriptor layer1;
    public AnimatedImageDescriptor layer2;
    public AnimatedImageDescriptor layer3;
    public string[] variables_set;
    public string[] variables_unset;
    public string text;
    public InteractionDescriptor[] interactions;
}


[System.Serializable]
public class InteractionDescriptor
{
    public string text;
    public ExitSceneDescriptor[] exit_scenes;
    public VariableRequirementsDescriptor requires_variables;
}

[System.Serializable]
public class MusicDescriptor
{
    public string track;
    public int volume;
    public bool looping;
}

[System.Serializable]
public class ExitSceneDescriptor
{
    public string scene;
    public float probability_weight;
    public bool exit_game;
    public string[] variables_set;
    public string[] variables_unset;
}

[System.Serializable]
public class VariableRequirementsDescriptor
{
    public string[] set;
    public string[] unset;
}

[System.Serializable]
public class AnimatedImageDescriptor
{
    public string image;
    public bool aniamtes;
    public float initialDelay;
    public float fadeInTime;
    public float sustainTime;
    public float fadeOutTime;
    public float repeatDelay;
    public bool loop;
}