#!/bin/bash

# Default
DEFAULT_UNITY_PATH="$HOME/Unity/Hub/Editor/2022.3.59f1/Editor/Unity"
DEFAULT_PROJECT_PATH="."
DEFAULT_TEST_MODE="editmode"
DEFAULT_RESULTS_FILE=""

# Show usage information
show_help() {
    cat << EOF
Usage: $0 [OPTIONS]

Unity test runner script

OPTIONS:
    -u, --unity-path PATH       Unity editor path (default: $DEFAULT_UNITY_PATH)
    -p, --project-path PATH     Project path (default: $DEFAULT_PROJECT_PATH)
    -m, --mode MODE             Test mode: editmode or playmode (default: $DEFAULT_TEST_MODE)
    -r, --results FILE          Results file name (default: results_<mode>.xml)
    -h, --help                  Show this help message

EXAMPLES:
    $0                                          # Run Editmode tests with default settings
    $0 -m playmode                              # Run Playmode tests
    $0 -u /path/to/Unity -m editmode           # Run Editmode tests with custom Unity path
    $0 -p /path/to/project -r my_results.xml   # Run tests with custom project path and results file

EOF
}

# Argument parsing
UNITY_PATH="$DEFAULT_UNITY_PATH"
PROJECT_PATH="$DEFAULT_PROJECT_PATH"
TEST_MODE="$DEFAULT_TEST_MODE"
RESULTS_FILE="$DEFAULT_RESULTS_FILE"

while [[ $# -gt 0 ]]; do
    case $1 in
        -u|--unity-path)
            UNITY_PATH="$2"
            shift 2
            ;;
        -p|--project-path)
            PROJECT_PATH="$2"
            shift 2
            ;;
        -m|--mode)
            TEST_MODE="$2"
            shift 2
            ;;
        -r|--results)
            RESULTS_FILE="$2"
            shift 2
            ;;
        -h|--help)
            show_help
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Run '$0 --help' to see available options."
            exit 1
            ;;
    esac
done

# Test mode validation
if [[ "$TEST_MODE" != "editmode" && "$TEST_MODE" != "playmode" ]]; then
    echo "Error: Test mode must be 'editmode' or 'playmode'."
    exit 1
fi

# Set results file name (if not specified)
if [[ -z "$RESULTS_FILE" ]]; then
    RESULTS_FILE="results_${TEST_MODE}.xml"
fi

# Check Unity editor existence
if [[ ! -f "$UNITY_PATH" ]]; then
    echo "Error: Unity editor not found: $UNITY_PATH"
    echo "Please specify the correct path with the -u option."
    exit 1
fi

# Check project path existence
if [[ ! -d "$PROJECT_PATH" ]]; then
    echo "Error: Project path not found: $PROJECT_PATH"
    exit 1
fi

# Show execution information
echo "=== Unity Test Runner ==="
echo "Unity Path: $UNITY_PATH"
echo "Project Path: $PROJECT_PATH"
echo "Test Mode: $TEST_MODE"
echo "Results File: $RESULTS_FILE"
echo "=========================="

# Run Unity tests
echo "Running tests..."
"$UNITY_PATH" -runTests -batchmode -projectPath "$PROJECT_PATH" -testResults "$RESULTS_FILE" -testPlatform "$TEST_MODE"

# Check execution result
exit_code=$?
if [[ $exit_code -eq 0 ]]; then
    echo "Tests completed successfully."
    echo "Results file: $RESULTS_FILE"
else
    echo "Tests failed. Exit code: $exit_code"
fi

exit $exit_code