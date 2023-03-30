using System;

public class XPSystem
{
    public static XPSystem Instance;
    int _currentLevel;
    int _currentXP;
    int _xpToNextLevel;

    public static Action<int> OnLevelChanged;
    public static Action<int, int> OnExperienceChanged;

    public XPSystem()
    {
        Instance = this;
        _currentLevel = 1;
        _currentXP = 0;
        _xpToNextLevel = 100;
        OnLevelChanged?.Invoke(_currentLevel);
        OnExperienceChanged?.Invoke(_currentXP, _xpToNextLevel);
    }

    public void AddExperience(int amount)
    {
        _currentXP += amount;
        while (_currentXP >= _xpToNextLevel) {
            _currentLevel++;
            _currentXP -= _xpToNextLevel;
            OnLevelChanged?.Invoke(_currentLevel);
            _xpToNextLevel = (int)(_currentLevel * 100 * 1.25);
        }
        OnExperienceChanged?.Invoke(_currentXP, _xpToNextLevel);
    }

    public int GetCurrentLevel() => _currentLevel;
}
