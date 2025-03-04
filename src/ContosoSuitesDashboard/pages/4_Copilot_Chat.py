import requests
import streamlit as st

st.set_page_config(layout="wide")

def send_message_to_copilot(message):
    """Send a message to the Copilot chat endpoint."""
    api_endpoint = st.secrets["api"]["endpoint"]
    response = requests.post(f"{api_endpoint}/MaintenanceCopilotChat", json=message, timeout=60)
    return response.text

def main():
    """Main function for the Maintenance Copilot Chat Streamlit page."""

    st.write(
        """
        # Maintenance Copilot chat

        This Streamlit dashboard is intended to demonstrate how you can use
        a Semantic Kernel agent to generate and save a maintenance request.

        ## Ask the Copilot to generate a maintenance request
        """
    )

    # Initialize chat history
    if "chat_messages" not in st.session_state:
        st.session_state.chat_messages = []

    # Display chat messages from history on app rerun
    for message in st.session_state.chat_messages:
        with st.chat_message(message["role"]):
            st.markdown(message["content"])

    # React to user input
    if prompt := st.chat_input("How I can help you today?"):
        with st.spinner("Awaiting the Copilot's response to your question..."):
            # Display user message in chat message container
            st.chat_message("user").markdown(prompt)
            # Add user message to chat history
            st.session_state.chat_messages.append({"role": "user", "content": prompt})
            # Send user message to Copilot and get response
            response = send_message_to_copilot(prompt)
            # Display assistant response in chat message container
            with st.chat_message("assistant"):
                st.markdown(response)
            # Add assistant response to chat history
            st.session_state.chat_messages.append({"role": "assistant", "content": response})


if __name__ == "__main__":
    main()        
