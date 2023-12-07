namespace AdventOfCode;

static class Approvals
{
    public static void Approve(string value, string outputFolder, string? scenarioName)
    {
        Extensions.EnsureFolder(outputFolder);

        var receivedFile = Path.Combine(outputFolder, $"{scenarioName}.received.txt");

        File.WriteAllText(receivedFile, value);

        var approvedFile = Path.Combine(outputFolder, $"{scenarioName}.approved.txt");

        if (!File.Exists(approvedFile))
        {
            File.WriteAllText(approvedFile, string.Empty);
        }

        var approvedText = File.ReadAllText(approvedFile);

        var normalizedApprovedText = approvedText.Replace("\r\n", "\n");
        var normalizedReceivedText = value.Replace("\r\n", "\n");

        if (!string.Equals(normalizedApprovedText, normalizedReceivedText))
        {
            throw new Exception("Approval verification failed.");
        }

        File.Delete(receivedFile);

    }
}

