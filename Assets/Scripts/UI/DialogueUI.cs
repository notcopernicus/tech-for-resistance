using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    // =========================
    // UI REFERENCES (Inspector)
    // =========================
    [Header("UI References (drag in Inspector)")]
    public GameObject dialoguePanel;      // The panel that contains everything
    public TMP_Text promptText;           // The big text area for subtitle/speaker
    public Button[] choiceButtons;        // Choice buttons (Size = 2 or 3)
    public Button continueButton;         // Continue button (for system nodes)
    public Button closeButton;

    // =========================
    // JSON FILES (Inspector)
    // =========================
    [Header("Scenario JSON (drag in Inspector)")]
    public TextAsset dialogueEN;          // English JSON TextAsset
    public TextAsset dialogueES;          // Spanish JSON TextAsset

    // =========================
    // INTERNAL STATE
    // =========================
    private Action<string> onChoiceSelected; // callback to Person A (returns choiceId)
    private ScenarioFile loadedFile;         // parsed JSON scenario
    private string currentNodeId;            // which node we are on right now

    // -------------------------
    // Unity lifecycle
    // -------------------------
    void Start()
    {
        // Hide UI at start
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // Continue hidden by default
        if (continueButton != null) continueButton.gameObject.SetActive(false);

        // Hide choice buttons by default
        if (choiceButtons != null)
        {
            foreach (var b in choiceButtons)
                if (b != null) b.gameObject.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseDialogue);
        }

        

    }

    // =========================
    // CONTRACT METHOD (Person A)
    // =========================
    // Person A calls this to open dialogue in a chosen language.
    // Example:
    // dialogueUI.ShowDialogue("en", (choiceId) => { Debug.Log(choiceId); });
    public void ShowDialogue(string language, Action<string> callback)
    {
        onChoiceSelected = callback;

        // 1) Choose correct language file
        TextAsset chosen = dialogueEN;
        string lang = (language ?? "en").ToLower().Trim();
        if (lang == "es") chosen = dialogueES;

        if (chosen == null)
        {
            Debug.LogError("Missing JSON TextAsset for language: " + language);
            return;
        }

        // 2) Parse JSON into C# objects
        loadedFile = JsonUtility.FromJson<ScenarioFile>(chosen.text);

        if (loadedFile == null || loadedFile.nodes == null || loadedFile.nodes.Length == 0)
        {
            Debug.LogError("Failed to parse JSON. Check keys/formatting.");
            return;
        }

        // 3) Start at the scenario's start node
        currentNodeId = loadedFile.startNodeId;

        if (string.IsNullOrEmpty(currentNodeId))
        {
            Debug.LogError("startNodeId is missing/empty in JSON.");
            return;
        }

        // 4) Show UI and render first node
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        DisplayCurrentNode();
    }

    // =========================
    // MAIN RENDER FUNCTION
    // =========================
    private void DisplayCurrentNode()
    {
        // Safety: clear/hide Continue every time (prevents old click listeners)
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(false);
            continueButton.onClick.RemoveAllListeners();
        }

        // Find node
        Node currentNode = FindNode(currentNodeId);
        if (currentNode == null)
        {
            Debug.LogError("Node not found: " + currentNodeId);
            return;
        }

        // Does this node have choices?
        bool hasChoices = currentNode.choices != null && currentNode.choices.Length > 0;

        // Set prompt text (speaker + subtitle)
        if (promptText != null)
        {
            if (!string.IsNullOrEmpty(currentNode.speaker))
                promptText.text = currentNode.speaker + ": " + currentNode.subtitle;
            else
                promptText.text = currentNode.subtitle;
        }

        // -------------------------
        // CASE 1: Node HAS choices
        // -------------------------
        if (hasChoices)
        {
            // Continue should be hidden when choices exist
            if (continueButton != null) continueButton.gameObject.SetActive(false);

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (choiceButtons[i] == null) continue;

                if (i < currentNode.choices.Length)
                {
                    Choice choice = currentNode.choices[i];
                    Button btn = choiceButtons[i];

                    btn.gameObject.SetActive(true);

                    // Label text on the button
                    TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
                    if (label != null) label.text = choice.text;

                    // Capture values for the click listener
                    string choiceId = choice.id;
                    string nextNodeId = choice.next;

                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => SelectChoice(choiceId, nextNodeId));
                }
                else
                {
                    choiceButtons[i].gameObject.SetActive(false);
                    choiceButtons[i].onClick.RemoveAllListeners();
                }
            }
        }
        // -------------------------
        // CASE 2: Node has NO choices
        // -------------------------
        else
        {
            // Hide all choice buttons
            foreach (var b in choiceButtons)
            {
                if (b == null) continue;
                b.gameObject.SetActive(false);
                b.onClick.RemoveAllListeners();
            }

            // Show Continue if there's a next node
            if (continueButton != null && !string.IsNullOrEmpty(currentNode.next))
            {
                string nextId = currentNode.next;

                continueButton.gameObject.SetActive(true);
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(() =>
                {
                    currentNodeId = nextId;
                    DisplayCurrentNode();
                });
            }
            // If node.next is empty/null, we're at an end node.
        }
    }

    // =========================
    // CHOICE CLICK HANDLER
    // =========================
    private void SelectChoice(string choiceId, string nextNodeId)
    {
        // Return choiceId to Person A (they can score/log it)
        onChoiceSelected?.Invoke(choiceId);

        // Advance to next node if specified
        if (!string.IsNullOrEmpty(nextNodeId))
        {
            currentNodeId = nextNodeId;
            DisplayCurrentNode();
        }
        else
        {
            // End
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }
    }

    // =========================
    // FIND NODE BY ID
    // =========================
    private Node FindNode(string nodeId)
    {
        if (loadedFile == null || loadedFile.nodes == null) return null;

        foreach (Node n in loadedFile.nodes)
        {
            if (n != null && n.id == nodeId) return n;
        }
        return null;
    }

    private void CloseDialogue()
{
    // Hide the panel
    if (dialoguePanel != null)
        dialoguePanel.SetActive(false);

    // Optional: clear state
    currentNodeId = null;

    // Optional: notify Person A that dialogue was closed
    onChoiceSelected?.Invoke("CLOSED");
}

    // =========================
    // JSON DATA CLASSES
    // (Keys must match JSON)
    // =========================
    [Serializable]
    public class ScenarioFile
    {
        public string scenarioId;
        public string startNodeId;
        public Node[] nodes;
    }

    [Serializable]
    public class Node
    {
        public string id;
        public string type;
        public string speaker;
        public string subtitle;
        public Choice[] choices;
        public string next; // used for system nodes or linear nodes
    }

    [Serializable]
    public class Choice
    {
        public string id;
        public string text;
        public string[] tags;
        public Impact impact;
        public string noteToPlayer;
        public string next; // IMPORTANT: JSON key is "next"
    }

    [Serializable]
    public class Impact
    {
        public int communication;
        public int confidence;
        public int equity;
    }


}
