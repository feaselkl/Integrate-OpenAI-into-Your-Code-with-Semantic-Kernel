import streamlit as st

st.set_page_config(layout="wide")

def main():
    st.write(
    """
    # Contoso Suites Main Page

    This Streamlit dashboard is intended to serve as a proof of concept of Azure OpenAI functionality for Contoso Suites employees.  It is not intended to be a production-ready application.

    Use the navigation bar on the left to navigate to the different pages of the dashboard.
    """
    )

if __name__ == "__main__":
    main()
